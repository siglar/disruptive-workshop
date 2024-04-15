namespace Exercise3.Hubs;

public interface ITemperatureHub
{
    Task SendTemperature(float temperature);
}