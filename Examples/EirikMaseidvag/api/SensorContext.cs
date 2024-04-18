#nullable disable

using Microsoft.EntityFrameworkCore;

namespace EirikMaseidvag;

public class SensorContext : DbContext
{
    public DbSet<Sensor> Sensors { get; set; }
    public DbSet<SensorValue> SensorValues { get; set; }

    public string DbPath { get; }

    public SensorContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "sensor.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

[Index(nameof(DeviceId), IsUnique = true)]
public class Sensor
{
    public int Id { get; init; }
    public string DeviceId { get; init; } 
    public List<SensorValue> Values { get; } = [];
}

public class SensorValue
{
    public int Id { get; init; }
    public float Temperature { get; init; }
    public DateTime TimeStamp { get; set; }
    public Sensor Sensor { get; set; }
    public int SensorId { get; set; }
}