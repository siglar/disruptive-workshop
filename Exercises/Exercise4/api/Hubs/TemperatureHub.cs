using Microsoft.AspNetCore.SignalR;

namespace Exercise4.Hubs;

public class TemperatureHub : Hub<ITemperatureHub>
{
    public async Task SendTemperature(float temperature)
    {
        await Clients.All.SendTemperature(temperature);
    }
}