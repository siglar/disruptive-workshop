using Exercise2.Hubs;
using Exercise2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Exercise2.Endpoints
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

            // Send the temperature value to all Websocket listeners
            await hubContext.Clients.All.SendTemperature(disruptiveData.Event.Data.Temperature.Value);

            // Disruptive will resend the event until it receives a 200 OK
            return TypedResults.Ok();
        }
    }
}
