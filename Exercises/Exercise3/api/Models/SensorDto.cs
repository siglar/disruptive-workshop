namespace Exercise3.Models
{
    public class DisruptiveTemperature
    {
        public float Value { get; init; }
        public DateTime UpdateTime { get; init; }
    }

    public class DisruptiveProximity
    {
        public string State { get; init; } = string.Empty;
        public DateTime UpdateTime { get; init; }
    }

    public class DisruptiveBatteryStatus
    {
        public int Percentage { get; init; }
        public DateTime UpdateTime { get; init; }
    }

    public class DisruptiveNetworkStatus
    {
        public int SignalStrength { get; init; }
        public int Rssi { get; init; }
        public DateTime UpdateTime { get; init; }
        public string TransmissionMode { get; init; } = string.Empty;
        public List<DisruptiveCloudConnector> CloudConnectors { get; init; } = [];
    }

    public class DisruptiveCloudConnector
    {
        public string Id { get; init; } = string.Empty;
        public int SignalStrength { get; init; }
        public int Rssi { get; init; }
    }

    public class DisruptiveTouch
    {
        public DateTime UpdateTime { get; init; }
    }

    public class Data
    {
        public DisruptiveTemperature? Temperature { get; init; }
        public DisruptiveTouch? Touch { get; init; }
        public DisruptiveProximity? ObjectPresent { get; init; }
        public DisruptiveBatteryStatus? BatteryStatus { get; init; }
        public DisruptiveNetworkStatus? NetworkStatus { get; init; }
        public DisruptiveCloudConnector? CloudConnector { get; init; }
    }

    public class Labels
    {
        public string Name { get; init; } = string.Empty;
    }

    public class DisruptiveData
    {
        public Event Event { get; init; } = new();
        public Labels Labels { get; init; } = new();

        public string GetDeviceName() {
            
            var fullName = Event.TargetName;
            var index = fullName.LastIndexOf('/');

            return index == -1 ? "" : fullName[(index + 1)..];
        }
    }

    public class Event
    {
        public string EventId { get; init; } = string.Empty;
        public string TargetName { get; init; } = string.Empty;
        public string EventType { get; init; } = string.Empty;
        public Data Data { get; init; } = new();
        public DateTime Timestamp { get; init; }
    }
}