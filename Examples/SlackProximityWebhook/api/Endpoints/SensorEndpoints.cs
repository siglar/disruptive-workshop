using System.Text;
using Exercise5.Hubs;
using Exercise5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Exercise5.Endpoints
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
            await using var db = new SensorContext();

            var temperatureValues = await db.SensorValues.OrderByDescending(sv => sv.TimeStamp).Where(sv => sv.Temperature != null).Take(100).ToListAsync();
            var proximityValues = await db.SensorValues.OrderByDescending(sv => sv.TimeStamp).Where(sv => sv.Proximity != null).Take(100).ToListAsync();

            var mappedTemperatures = temperatureValues.Select(sv => new TemperatureViewModel(sv.Temperature ?? 0, sv.TimeStamp));
            var mappedProximities = proximityValues.Select(sv => new ProximityViewModel(sv.Proximity, sv.TimeStamp));
            var sensors = new SensorsViewModel(mappedTemperatures, mappedProximities);


            return TypedResults.Ok(sensors);
        }

        private static async Task<IResult> Post(
            [FromBody] DisruptiveData disruptiveData,
            IHubContext<TemperatureHub, ITemperatureHub> hubContext
            )
        {
            if (disruptiveData.Event.Data.Temperature is null && disruptiveData.Event.Data.ObjectPresent is null)
            {
                // We only handle temperature events
                return TypedResults.Ok();
            }


            // Save current value to a SQLite database
            await SaveToDatabase(disruptiveData);

            // Send the temperature value to all Websocket listeners
            if (disruptiveData.Event.Data.Temperature is not null)
            {
                var temperatureViewModel = new TemperatureViewModel(
                    disruptiveData.Event.Data.Temperature.Value,
                    disruptiveData.Event.Timestamp
                );

                await hubContext.Clients.All.SendTemperature(temperatureViewModel);
            }

            // Disruptive will resend the event until it receives a 200 OK
            return TypedResults.Ok();
        }

        private static async Task SaveToDatabase(DisruptiveData disruptiveData)
        {
            await using var db = new SensorContext();

            var sensor = await db.Sensors.SingleOrDefaultAsync(s => s.DeviceId == disruptiveData.GetDeviceName());

            SensorValue value = new SensorValue
            {
                Temperature = null,
                TimeStamp = DateTime.Now
            };

            if (disruptiveData.Event.Data.Temperature is not null)
            {
                value = new SensorValue
                {
                    Temperature = disruptiveData.Event.Data.Temperature.Value,
                    TimeStamp = disruptiveData.Event.Timestamp
                };
            }

            if (disruptiveData.Event.Data.ObjectPresent is not null)
            {
                value = new SensorValue
                {
                    Proximity = disruptiveData.Event.Data.ObjectPresent.State,
                    TimeStamp = disruptiveData.Event.Timestamp
                };
            }


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

            // Print the values in log for debugging
            if (value.Proximity is not null)
            {
                await NotifySlack(disruptiveData);
                Console.WriteLine($"Device: {sensor.DeviceId} - Proximity: {value.Proximity} - Time: {value.TimeStamp}");
            }
        }

        private static async Task NotifySlack(DisruptiveData disruptiveData)
        {
            await using var db = new SensorContext();

            // If this is the first time proximity is detected and the case is closed, notify slack
            var proximityValues = await db.SensorValues
                .OrderByDescending(sv => sv.TimeStamp)
                .Where(sv => sv.Proximity != null)
                // Only check last ten minutes
                //.Where(sv => sv.TimeStamp > DateTime.Now.AddMinutes(-1))
                .Take(10).ToListAsync();
            if (proximityValues.Count > 0 && proximityValues[0].Proximity == "NOT_PRESENT")
            {
                Console.WriteLine("Lid opened for first time");
                var webhookUrl = "https://hooks.slack.com/services/<your-webhook-url>";

                // Create an HttpClient
                using (var client = new HttpClient())
                {
                    // Create the JSON payload
                    var payload = new StringContent("{\"text\": \"It's beer o'clock!\"}",
                                                    Encoding.UTF8,
                                                    "application/json");

                    // Post the payload to the webhook
                    HttpResponseMessage response = await client.PostAsync(webhookUrl, payload);

                    // Check the result
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Message sent successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to send message. Status Code: " + response.StatusCode);
                    }
                }
            }
        }
    }
}
