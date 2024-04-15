import { FC, useEffect, useState } from "react";
import Chart from "../components/Chart.component";
import { TemperatureViewModel } from "../models/temperature.viewmodel";
import Connector from "../signalr-connection";

const { events } = Connector();

const LandingPage: FC = () => {
  const [sensorValues, setSensorValues] = useState<TemperatureViewModel[]>([]);

  // Fetch 10 last temperature values on render
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

  useEffect(() => {
    // Callback that is fired when a new temperature is received from the Websocket
    events((temperature) =>
      setSensorValues((current) => [...current.slice(-9), temperature])
    );
  });

  return (
    <>
      {sensorValues.length > 0 && (
        <>
          <p>
            Current temperature:{" "}
            {sensorValues[sensorValues.length - 1].temperature}°C
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
