using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints
{
    public static class Bookings
    {
        public static void RegisterSensorEndpoints(this IEndpointRouteBuilder routes)
        {
            var sensorGroup = routes.MapGroup("").WithTags("Sensor");

            sensorGroup
                .MapPost("", Post)
                .WithOpenApi();
        }

        private static IResult Post([FromBody] DisruptiveData disruptiveData)
        {
            if (disruptiveData.Event.Data.Temperature is null)
            {
                // We only handle temperature events
                return TypedResults.Ok();
            }

            Console.WriteLine($"Hello {disruptiveData.Labels.Name}. This is your sensor 🌡️");
            Console.WriteLine($"The current temperature is {disruptiveData.Event.Data.Temperature.Value}" + "️\u00b0C");
            return TypedResults.Ok();
        }
    }
}
