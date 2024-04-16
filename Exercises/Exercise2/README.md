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

In the `src` directory create a folder directory called `pages`. In the `pages` directory create a file called `Landing.page.tsx`. Paste the following content:

```typescript
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
```

This React component will be our landing page. This file now contains what we need to listen for messages from our SignalR hub. We say that there is an event called `SendTemperature` which will contain numeric value. This is actually a websocket connection broadcasting our temperature.

Navigate to `App.tsx` and replace the content with:

```typescript
import { FC } from "react";
import "./App.css";
import LandingPage from "./pages/Landing.page";

const App: FC = () => {
  return (
    <div className="App">
      <LandingPage />
    </div>
  );
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

Continue to [Exercise3](../Exercise3/README.md).
