using Exercise5.Models;

namespace Exercise5.Hubs;

public interface ITemperatureHub
{
    Task SendTemperature(TemperatureViewModel temperature);
}