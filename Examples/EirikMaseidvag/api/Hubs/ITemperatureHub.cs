using EirikMaseidvag.Models;

namespace EirikMaseidvag.Hubs;

public interface ITemperatureHub
{
    Task SendTemperature(TemperatureViewModel temperature);

    Task CoffeeReady();

    Task Proximity(bool isOpen);
}