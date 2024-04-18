using EirikMaseidvag.Models;
using Microsoft.AspNetCore.SignalR;

namespace EirikMaseidvag.Hubs;

public class TemperatureHub : Hub<ITemperatureHub>
{
    public async Task SendTemperature(TemperatureViewModel temperature)
    {
        await Clients.All.SendTemperature(temperature);
    }

    public async Task CoffeeReady()
    {
        await Clients.All.CoffeeReady();
    }

    public async Task Proximity(bool isOpen)
    {
        await Clients.All.Proximity(isOpen);
    }
}