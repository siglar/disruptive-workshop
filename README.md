# Disruptive Workshop

Welcome to the Disruptive workshop! This repo contains examples and documentation to be used in the workshop.
Disruptive has written an extensive amount of documentation located here.

# 1.0 - Setting up the API

The API is written in C# with .NET 8. Therefore there are a couple of requirements to get it up and running:

- Install the .NET 8 SDK. Find and download (the newest version) [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
- Write `dotnet --info` in a terminal to check that it successfully installed. Might have to restart the terminal first.

In a terminal, navigate to the `SensorAPI` directory in this repo and execute `dotnet run`. The API should now be running on [http://localhost:5124](http://localhost:5124). Swagger should be exposed on [http://localhost:5124/swagger](http://localhost:5124/swagger).

# 1.1 - The API

The API is [.NET minimal API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-8.0&tabs=visual-studio). Minimal APIs were introduced in .NET 6 and offers a lightweight alternative to the standard controller based API.
