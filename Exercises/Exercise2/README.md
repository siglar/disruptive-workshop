# 2.0 - Forwarding data to a front-end

> [!IMPORTANT]
> For this exercise you'll need to install [Node.js](https://nodejs.org/en).
> Install the latest LTS version before continuing.

In this exercise we'll expand our solution with a front-end that'll receive real time data from our API.

Let's start by scaffolding a project with [Vite](https://vitejs.dev/guide/).

In the `Exercise2` directory, run: `npm create vite@latest frond-end -- --template react-ts`

Navigate to the newly created `frond-end` directory and run `npm i` and then `npm run dev` (or just `npm i && npm run dev` if your terminal supports bash)

You should now have React app running on your localhost.

# 2.1 - Connect to a SignalR hub

Start by navigation to this exercises [api](./api/) directory. Run `dotnet watch` to start the api. Keep it running in the background.

In another terminal instance, navigate to the `front-end` directory and install SignalR: `npm i @microsoft/signalr`

Open the front-end in your favorite code editor (I use VSCode).

In the `src` directory create a file called: `signalr-connection.ts`. Paste the following content into the file:

```typescript
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
```

This file now contains what we need to listen for messages from our SignalR hub. We say that there is an event called `SendTemperature` which will contain numeric value. This is actually a websocket connection broadcasting our temperature.

In the `src` directory create a folder directory called `pages`. In the `pages` directory create a file called `Landing.page.tsx`. Paste the following content:

```typescript
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
```

This React component will be our landing page. On initialization it'll create a `Connnector` where we can listen for events. When we receive the event we'll render it in the component.

Navigate to `App.tsx` and replace the content with:

```typescript
import { FC } from "react";
import "./App.css";
import LandingPage from "./pages/Landing.page";

const App: FC = () => {
  return <LandingPage />;
};

export default App;
```

Replace the content of `App.css` with just:

```css
#root {
  max-width: 1280px;
  margin: 0 auto;
  padding: 2rem;
  text-align: center;
}
```

Now do `npm run dev` again. Press the sensor. If your lucky, the temperature should now show in the web application!

If not, make sure that:

- Your API is running.
- ngrok is running (`ngrok http 1337`)
- The correct ngrok URL is pasted into your Disruptive data connector.
