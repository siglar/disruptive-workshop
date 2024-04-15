using Exercise4.Hubs;
using Exercise4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Exercise4.Endpoints
{
    public static class SensorEndpoints
    {
        public static void RegisterSensorEndpoints(this IEndpointRouteBuilder routes)
        {
            var sensorGroup = routes.MapGroup("").WithTags("Sensor");

            sensorGroup
                .MapPost("", Post)
                .WithOpenApi();
        }

        private static async Task<IResult> Post(
            [FromBody] DisruptiveData disruptiveData,
            IHubContext<TemperatureHub, ITemperatureHub> hubContext
            )
        {
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
            await hubContext.Clients.All.SendTemperature(disruptiveData.Event.Data.Temperature.Value);

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
    }
}
