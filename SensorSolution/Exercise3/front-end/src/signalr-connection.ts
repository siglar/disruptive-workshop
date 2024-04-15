import * as signalR from "@microsoft/signalr";
const URL = "http://localhost:1337/temperatureHub";

class Connector {
  private connection: signalR.HubConnection;

  public events: (sendTemperature: (temperature: number) => void) => void;

  static instance: Connector;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(URL)
      .withAutomaticReconnect()
      .build();

    this.connection.start().catch((err) => document.write(err));

    this.events = (sendTemperature) => {
      this.connection.on("sendTemperature", (temperature) => {
        sendTemperature(temperature);
      });
    };
  }

  public static getInstance(): Connector {
    if (!Connector.instance) Connector.instance = new Connector();
    return Connector.instance;
  }
}
export default Connector.getInstance;
