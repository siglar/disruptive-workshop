# 1.0 - Forwarding data to a front-end

Simply printing the temperature in a terminal is cool and all, but let's forward it somewhere a bit more user-friendly: a web application.

<!-- I'll give you two choices, choose one:

1. Scaffold a front-end in this directory with [Vite](https://vitejs.dev/guide/).

Preferably use a React with Typescript. So do: `npm create vite@latest front-end -- --template react-ts`  -->

Before we implement a front-end though, we need to somehow forward the temperature data from our API to whoever is listening. This is where [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr) will help us.

SignlaR is a ASP.NET library for creating [websockets](https://en.wikipedia.org/wiki/WebSocket). A websocket lets us communicate two ways over a single TCP connection. This becomes useful for us as we want to create a front-end that <i>listens</i> for changes (as opposed to a front-end that constantly <i>asks</i> for any changes).

# 1.1 Implementing SignalR

Edit your Program.cs to look like this:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddCors();

var app = builder.Build();

app.MapHub<TemperatureHub>("/temperatureHub");

app.RegisterSensorEndpoints();
app.UseSwagger();
app.UseSwaggerUI();

// Very non-recommended CORS settings.
// We need to add it to make SignalR work.
// Please do not use in production.
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.Run();
```

Here we've added the SignalR service and a "hub". CORS is required, so we add that as well. For the sake of the workshop, we configure CORS to basically let anything through.

A "hub" is SignalR's way of sorting related methods. So anybody connecting to our "temperatureHub" can expect to receive data about temperature related events. We can create as many hubs as we want, but for now we'll stick to one.

Next, create a directory called `Hubs` in the located in the `Exercise1` directory. Add two files to the new directory: an interface called `ITemperatureHub.cs` and a class called `TemperatureHub.cs`

`ITemperatureHub.cs` should look like this:

```csharp
namespace Exercise1.Hubs;

public interface ITemperatureHub
{
    Task SendTemperature(float temperature);
}
```

`TemperatureHub.cs` should look like this:

```csharp
using Microsoft.AspNetCore.SignalR;

namespace Exercise1.Hubs;

public class TemperatureHub : Hub<ITemperatureHub>
{
    public async Task SendTemperature(float temperature)
    {
        await Clients.All.SendTemperature(temperature);
    }
}
```

This code implements a method in the `TemperatureHub` that allows us to broadcast the temperature to all listening clients.

Now navigate to the [SensorEndpoint]('./Endpoints/SensorEndpoints.cs') file and modify the Post method to like this:

```csharp
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
```

Here we inject our newly created `TemperatureHub` and uses its `SendTemperature` method to broadcast the temperature. Now we just need somebody to listen...
