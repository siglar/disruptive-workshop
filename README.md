# Disruptive Workshop

Welcome to the Disruptive workshop! This repo contains examples and documentation to be used in the workshop.
Disruptive has written an extensive amount of documentation located [here](https://disruptive.gitbook.io/docs).

# 1.0 - Setting up the API

The API is written in C# with .NET 8. Therefore there are a couple of requirements to get it up and running:

- Install the .NET 8 SDK. Find and download (the newest version) [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
- Write `dotnet --info` in a terminal to check that it successfully installed. Might have to restart the terminal first.

In a terminal, navigate to the `API` directory in this repo and execute `dotnet watch`. The API should now be running on [http://localhost:5047](http://localhost:5047) (the port might be different, so take not of that). Swagger should be exposed on [http://localhost:5047/swagger](http://localhost:5047/swagger). Since we ran `dotnet watch` the code should auto recompile on changes, but it's still a bit janky, so you can do `cmd + r` or `ctrl + r` in the terminal to force it to recompile.

Keep the API running and let's continue.

# 1.1 - The API

The API is a [.NET minimal API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-8.0&tabs=visual-studio). Minimal APIs were introduced in .NET 6 and offers a lightweight alternative to the standard controller based API. Instead of controller files, we now simply have endpoints. In this particular API, there is one endpoint which is located in the `SensorEndpoints.cs` file.

This endpoint (let's call it the sensor-endpoint) receives sensor data from Disruptive. It obviously does not do this without som configuration, so let's do that.

# 1.2 - The Disruptive Portal

You should all have been given access to the [Disruptive portal](https://studio.disruptive-technologies.com/). Shout out if you haven't. Log in with your credentials and choose the project with your name. In the `Sensors & Cloud Connectors` page you can see all the devices that are configurable in this project. You should have a sensor named after you. That's neat!

Now, navigate to `API Integrations` and the choose `Data connectors`. Again, here you'll have a connector named after you. Doubly neat!

Simply put; a data connector is a webhook. You need to configure it to forward data from a sensor to an endpoint. We want it to send data to our locally hosted APIs (i.e. our localhost). We can do this with a proxy. That takes us to our next step.

# 1.3 Install ngrok

ngrok is a free proxy service that enables forwarding HTTPS traffic from a publicly accessible URL to a port on localhost. So we can use it to create an online endpoint that'll forward data to our computers. Go to their [download page](https://ngrok.com/download) to find instructions on how to download and install it.

Sadly, they now require an account to use it. Without an account you wont be able to obtain an access token. So go ahead and create a user account on their website. When you've done this (and installed it) log in an copy the command that contains your token. It should look something like this: `ngrok config add-authtoken <your_token>`

Now your ready to create a proxy. To do so, run: `ngrok http 5047`. The number here is your localhost port, so change it if it differs from the example.

If successful, you'll now have a proxy. It should look like this:
![proxy](./Images/proxy.jpg)

Leave ngrok running and copy the URL. One important note to make: <i> restarting ngrok will produce a different URL. </i> Therefore, try to keep it running throughout the whole workshop.

Now that you have a URL, return to the Disruptive portal and navigate to your data connector. Paste the URL in the `Endpoint URL` textfield and append `/Sensor` on the end. Like this:

![alt text](./Images/details.png)

So the URL should look something like `https://just-an-example-do-not-use.ngrok-free.app/Sensor`. Remember the `/Sensor` part!

Now enable your data connector. It's the switch that looks like this:

![alt text](./Images/enable.png)

Scroll to the bottom of the page and click `Update data connector`

![alt text](./Images/update.png)

When this is done, return to where your API is still (hopefully) running. Is it printing sweet, sweet data? If yes, celebrations! 🎉 If no, return to start... 😡 Or call out, and I'll come and help you.
