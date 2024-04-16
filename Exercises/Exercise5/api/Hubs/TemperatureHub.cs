using Exercise5.Models;
using Microsoft.AspNetCore.SignalR;

namespace Exercise5.Hubs;

public class TemperatureHub : Hub<ITemperatureHub>
{
    public async Task SendTemperature(TemperatureViewModel temperature)
    {
        await Clients.All.SendTemperature(temperature);
    }
}