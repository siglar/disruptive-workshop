import { FC, useEffect, useState } from "react";
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from "@microsoft/signalr";

const LandingPage: FC = () => {
  const [connection, setConnection] = useState<null | HubConnection>(null);
  const [currentTemperature, setCurrentTemperature] = useState<number>(0);

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
            setCurrentTemperature(temperature);
          });
        })
        .catch((error) => console.log(error));
    }
  }, [connection]);

  return (
    <span>
      Current temperature:{" "}
      <span style={{ color: "green" }}>{currentTemperature}</span>
    </span>
  );
};

export default LandingPage;
