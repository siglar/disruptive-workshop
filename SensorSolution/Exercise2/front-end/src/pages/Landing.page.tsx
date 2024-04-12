import { FC, useEffect, useState } from "react";
import Connector from "../signalr-connection";

const LandingPage: FC = () => {
  const { events } = Connector();

  const [currentTemperature, setCurrentTemperature] = useState<number>(0);

  useEffect(() => {
    events((temperature) => setCurrentTemperature(temperature));
  });

  return (
    <div className="App">
      <span>
        Current temperature:{" "}
        <span style={{ color: "green" }}>{currentTemperature}</span>
      </span>
    </div>
  );
};

export default LandingPage;
