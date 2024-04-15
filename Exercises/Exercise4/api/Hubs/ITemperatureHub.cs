namespace Exercise4.Hubs;

public interface ITemperatureHub
{
    Task SendTemperature(float temperature);
}