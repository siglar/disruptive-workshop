# Slack Webhook for Proximity Sensor

This dotnet project is a simple example of how to use a Slack webhook to send messages to a Slack channel when a proximity sensor is triggered.
In our case we wanted to use this to notify us when the bar cask is opened and it is beer o'clock 🍺.

The code includes the support for listing values for both proximity and temperature readings from Disruptive Technologies sensors.
While not implemented here the idea was to only trigger the beer o'clock message if the temperature was below a certain threshold. Who wants lukewarm beer?
The endpoint to get sensor data would also list both proximity and temperature values to allow a frontend to show a beer o'clock status based on the data.