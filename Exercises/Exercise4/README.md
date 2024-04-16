# 4.0 - Visualizing the data

Now that we we've stored the data, let's add some code to visualize it. Firstly we need to do some minor changes to the back-end.

Let's move into the [SensorEndpoints.cs](api/Endpoints/SensorEndpoints.cs) file again. We need to add an endpoint to retrieve our stored data. Start by editing the `RegisterSensorEndpoints` method:

```csharp
public static void RegisterSensorEndpoints(this IEndpointRouteBuilder routes)
{
    var sensorGroup = routes.MapGroup("").WithTags("Sensor");

    sensorGroup
        .MapGet("", Get)
        .WithOpenApi();

    sensorGroup
        .MapPost("", Post)
        .WithOpenApi();
}
```

Then add a method in the `SensorEndpoints` class:

```csharp
private static async Task<IResult> Get() {
    await using var db = new SensorContext();

    var databaseValues = await db.SensorValues.OrderByDescending(sv => sv.TimeStamp).Take(10).ToListAsync();

    var returnValues = databaseValues.Select(sv => new TemperatureViewModel(sv.Temperature, sv.TimeStamp));

    return TypedResults.Ok(returnValues.OrderBy(r => r.Timestamp));
}
```

This simply adds a new GET endpoint. The endpoint returns the 10 most recent temperature values.

Now move into the [Models](api/Models/) directory and add a file called `TemperatureViewModel.cs`. Add the following code:

```csharp
public record TemperatureViewModel(float Temperature, DateTime Timestamp);
```

Now we want to expose this data model via our SignalR hub instead of just a float value. So move into to the [Hubs](api/Hubs/) directory and edit both `ITemperatureHub.cs` and `TemperatureHub.cs` to use the new model:

`SendTemperature` in `ITemperatureHub.cs` should now be:

```csharp
Task SendTemperature(TemperatureViewModel temperature);
```

`SendTemperature` in `TemperatureHub.cs` should be:

```csharp
public async Task SendTemperature(TemperatureViewModel temperature)
{
    await Clients.Client(Context.ConnectionId).SendTemperature(temperature);
}
```

Go back to [SensorEndpoints.cs](api/Endpoints/SensorEndpoints.cs) and edit this line:

```csharp
await hubContext.Clients.All.SendTemperature(disruptiveData.Event.Data.Temperature.Value);
```

To use our new model:

```csharp
var temperatureViewModel = new TemperatureViewModel(
                disruptiveData.Event.Data.Temperature.Value,
                disruptiveData.Event.Timestamp
            );

await hubContext.Clients.All.SendTemperature(temperatureViewModel);
```

We've now edited our SignalR endpoint to send a message with both the temperature and a timestamp. We'll need this in the next task.

# 4.1 Adding a chart to the front-end

Let's focus on the front-end from now on. Move into the current [front-end](front-end/) directory. Let's install a chart library called [Recharts](https://recharts.org): `npm install recharts`

Recharts contains a bunch of different chart types, plus it's all just React components which makes it easy to work with. We're interested in the `LineChart` component to render our data.

Start off by adding a directory called `models` in the `src` directory. Add a file called `temperature.viewmodel.ts`. Add the following code to this file:

```typescript
export interface TemperatureViewModel {
  temperature: number;
  timestamp: string;
}
```

Again in the `src` directory, add a new directory called `components`. Add a file called `Chart.component.tsx` in this new directory. Paste the following code into our new component:

```typescript
import { FC } from "react";
import { LineChart, Line, XAxis, YAxis, Tooltip, TooltipProps } from "recharts";
import { TemperatureViewModel } from "../models/temperature.viewmodel";
import {
  ValueType,
  NameType,
} from "recharts/types/component/DefaultTooltipContent";

interface ChartProps {
  values: TemperatureViewModel[];
}

const Chart: FC<ChartProps> = (props: ChartProps) => {
  const renderTooltip = (content: TooltipProps<ValueType, NameType>) => {
    if (content.payload && content.payload.length > 0) {
      return (
        <article
          style={{
            border: "#bbb 1.5px solid",
          }}
        >
          <p
            style={{
              margin: "0 0",
              padding: "3px 7.5px",
              borderBottom: "#bbb 1.5px solid",
            }}
          >
            {content.payload[0].payload.temperature}°C
          </p>
        </article>
      );
    }
    return null;
  };

  const formatDate = (date: string) => {
    const currentDate = new Date(date);

    return `${currentDate.getHours()}:${currentDate.getMinutes()}:${currentDate.getSeconds()}`;
  };

  return (
    <LineChart width={1000} height={300} data={props.values}>
      <Line
        animationEasing={"linear"}
        animationDuration={250}
        type="monotone"
        dataKey="temperature"
        stroke="#8884d8"
      />
      <Tooltip content={(content) => renderTooltip(content)} />
      <XAxis dataKey="timestamp" tickFormatter={formatDate} />
      <YAxis />
    </LineChart>
  );
};

export default Chart;
```

This component now contains code to render a line chart. You can read the documentation for the line chart component [here](https://recharts.org/en-US/api/LineChart).

Now navigate to the [Landing page component.](front-end/src/pages/Landing.page.tsx). This component should now look like this:

```typescript
import { FC, useEffect, useState } from "react";
import Chart from "../components/Chart.component";
import { TemperatureViewModel } from "../models/temperature.viewmodel";
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from "@microsoft/signalr";

const LandingPage: FC = () => {
  const [sensorValues, setSensorValues] = useState<TemperatureViewModel[]>([]);
  const [connection, setConnection] = useState<null | HubConnection>(null);

  // Fetch 10 last temperature values on component mount
  useEffect(() => {
    const api = async () => {
      const data = await fetch("http://localhost:1337", {
        method: "GET",
      });
      const jsonData = (await data.json()) as TemperatureViewModel[];

      setSensorValues(jsonData);
    };

    api();
  }, []);

  // Connect to temperatureHub websocket on component mount
  useEffect(() => {
    const connect = new HubConnectionBuilder()
      .withUrl("http://localhost:1337/temperatureHub")
      .withAutomaticReconnect()
      .build();

    setConnection(connect);
  }, []);

  // Callback that is triggered when we receive a temperature
  useEffect(() => {
    if (connection) {
      if (connection.state == HubConnectionState.Connected) return;

      connection
        .start()
        .then(() => {
          connection.on("sendTemperature", (temperature) => {
            setSensorValues((current) => [...current.slice(-9), temperature]);
          });
        })
        .catch((error) => console.log(error));
    }
  }, [connection]);

  return (
    <>
      {sensorValues.length > 0 && (
        <>
          <p>
            Current temperature:{" "}
            <span style={{ color: "green" }}>
              {sensorValues[sensorValues.length - 1].temperature}°C
            </span>
          </p>
          <div className="App">
            {sensorValues && <Chart values={sensorValues} />}
          </div>
        </>
      )}
    </>
  );
};

export default LandingPage;
```

A couple of things have changed here. The first `UseEffect` is triggered once when the component renders. Its job is to fetch the last 10 temperature values. It uses the GET endpoint we created in the last task.

The second `UseEffect` remains unchanged.

The third `UseEffect` is still our Websocket callback handler. But now it receives our new data model `(TemperatureViewModel)` and adds it to the end of our currently displayed temperature values. This will again trigger the chart component to rerender and add our newly received value to the end of the chart.

Now let's start everything up again. So:

- `dotnet watch` the backend
- make sure `ngrok` is running
- make sure the Disruptive data connector has the right URL
- run `npm run dev` to start the front end.

If all is good you'll now have a line chart in your front-end. Trigger your sensor to send data and see if the chart auto updates.

Continue to [Exercise5](../Exercise5/README.md)
