using Exercise4.Models;
using Microsoft.AspNetCore.SignalR;

namespace Exercise4.Hubs;

public class TemperatureHub : Hub<ITemperatureHub>
{
    public async Task SendTemperature(TemperatureViewModel temperature)
    {
        await Clients.Client(Context.ConnectionId).SendTemperature(temperature);
    }
}