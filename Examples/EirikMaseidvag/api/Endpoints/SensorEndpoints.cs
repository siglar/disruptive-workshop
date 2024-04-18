using EirikMaseidvag.Hubs;
using EirikMaseidvag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EirikMaseidvag.Endpoints
{
    public static class SensorEndpoints
    {
        public static void RegisterSensorEndpoints(this IEndpointRouteBuilder routes)
        {
            var sensorGroup = routes.MapGroup("").WithTags("Sensor");

            sensorGroup
                .MapGet("", Get)
                .WithOpenApi();

            sensorGroup
                .MapPost("", Post)
                .WithOpenApi();
        }

        private static async Task<IResult> Get()
        {
            var returnValues = await getLatestTemperatureMeasurements();

            return TypedResults.Ok(returnValues);
        }

        // list of previous event ids
        private static List<string> previousEventIds = new();

        private static async Task<IResult> Post(
            [FromBody] DisruptiveData disruptiveData,
            IHubContext<TemperatureHub, ITemperatureHub> hubContext
            )
        {

            if (disruptiveData.Event.Data.ObjectPresent is not null)
            {
                if (previousEventIds.Contains(disruptiveData.Event.EventId))
                {
                    // We have already processed this event
                    return TypedResults.Ok();
                }

                previousEventIds.Add(disruptiveData.Event.EventId);

                await hubContext.Clients.All.Proximity(disruptiveData.Event.Data.ObjectPresent.State == "NOT_PRESENT");
            }

            if (disruptiveData.Event.Data.Temperature is null)
            {
                // We only handle temperature events
                return TypedResults.Ok();
            }

            Console.WriteLine($"Hello {disruptiveData.Labels.Name}. This is your sensor calling.");
            Console.WriteLine($"The current temperature is {disruptiveData.Event.Data.Temperature.Value}" + "️\u00b0C");

            // Save current value to a SQLite database
            await SaveToDatabase(disruptiveData);

            // Send the temperature value to all Websocket listeners
            var temperatureViewModel = new TemperatureViewModel(
                disruptiveData.Event.Data.Temperature.Value,
                disruptiveData.Event.Timestamp
            );

            await hubContext.Clients.All.SendTemperature(temperatureViewModel);

            await checkIsCoffeeReady(hubContext);

            // Disruptive will resend the event until it receives a 200 OK
            return TypedResults.Ok();
        }

        private static async Task SaveToDatabase(DisruptiveData disruptiveData)
        {
            await using var db = new SensorContext();

            var sensor = await db.Sensors.SingleOrDefaultAsync(s => s.DeviceId == disruptiveData.GetDeviceName());

            var value = new SensorValue
            {
                Temperature = disruptiveData.Event.Data.Temperature!.Value,
                TimeStamp = disruptiveData.Event.Timestamp
            };

            if (sensor is null)
                sensor = new Sensor
                {
                    DeviceId = disruptiveData.GetDeviceName(),
                    Values = { value }
                };

            else sensor.Values.Add(value);

            db.Sensors.Update(sensor);
            await db.SaveChangesAsync();

            Console.WriteLine("Value saved successfully 🎉");
        }

        private static async Task<IEnumerable<TemperatureViewModel>> getLatestTemperatureMeasurements()
        {

            await using var db = new SensorContext();

            var databaseValues = await db.SensorValues.OrderByDescending(sv => sv.TimeStamp).Take(10).ToListAsync();

            var returnValues = databaseValues.Select(sv => new TemperatureViewModel(sv.Temperature, sv.TimeStamp));

            return returnValues.OrderBy(r => r.Timestamp);
        }

        private static async Task checkIsCoffeeReady(IHubContext<TemperatureHub, ITemperatureHub> hubContext)
        {
            var latest = await getLatestTemperatureMeasurements();

            if (latest.Count() < 2)
            {
                return;
            }

            var last = latest.Last();
            var secondLast = latest.ElementAt(latest.Count() - 2);

            if (last is null || secondLast is null)
            {
                return;
            }

            if (last.Temperature > 60 && secondLast.Temperature > last.Temperature + 1)
            {
                await hubContext.Clients.All.CoffeeReady();
            }
        }
    }
}
