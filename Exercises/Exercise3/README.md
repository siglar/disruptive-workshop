# 3.0 - Storing the data

In this exercise we'll store the incoming data in a SQLite database. For simplicity, we'll do this using [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/).

Navigate to this exercises [api directory](./api/). Run the following command: `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`

In the same directory create a file called `SensorContext.cs`. Paste in the following content:

```csharp
#nullable disable

using Microsoft.EntityFrameworkCore;

namespace Exercise4;

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
```

This code represents our database. Our database contains to tables: `Sensors` and `SensorValues`.

Go back to your terminal (still in the `api` directory) and run the following commands:

```bash
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update
```

You've now scaffolded a database we can use.

Now navigate to the [SensorEndpoints](./api/Endpoints/SensorEndpoints.cs) file. Add the following method in the class:

```csharp
private static async Task SaveToDatabase(DisruptiveData disruptiveData)
{
    await using var db = new SensorContext();

    var sensor = await db.Sensors.SingleOrDefaultAsync(s => s.DeviceId == disruptiveData.GetDeviceName());

    var value = new SensorValue
    {
        Temperature = disruptiveData.Event.Data.Temperature!.Value,
        TimeStamp = disruptiveData.Event.Timestamp
    };

    if (sensor is null)
        sensor = new Sensor
        {
            DeviceId = disruptiveData.GetDeviceName(),
            Values = { value }
        };

    else sensor.Values.Add(value);

    db.Sensors.Update(sensor);
    await db.SaveChangesAsync();

    Console.WriteLine("Value saved successfully 🎉");
}
```

Then add `await SaveToDatabase(disruptiveData);` somewhere in the Post method.

The `SaveToDatabase` does what the names says. It checks if the sensor already exists in the database, and saves the new incoming value. If the sensor does not exits, it creates and adds the value in one go.

Now, run your `api` with `dotnet watch` and press your sensor a couple of times. You should see a message celebrating your great achievement 🎉

Continue to [Exercise4](../Exercise4/README.md)
