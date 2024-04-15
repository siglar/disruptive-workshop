namespace Exercise2.Hubs;

public interface ITemperatureHub
{
    Task SendTemperature(float temperature);
}