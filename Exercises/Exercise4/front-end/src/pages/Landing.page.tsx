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
