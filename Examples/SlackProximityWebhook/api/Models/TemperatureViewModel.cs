namespace Exercise5.Models;

public record TemperatureViewModel(float Temperature, DateTime Timestamp);

public record ProximityViewModel(string Proximity, DateTime Timestamp);

public record SensorsViewModel(IEnumerable<TemperatureViewModel> Temperatures, IEnumerable<ProximityViewModel> proximities);