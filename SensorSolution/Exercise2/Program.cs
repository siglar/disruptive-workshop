using Exercise2.Endpoints;
using Exercise2.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddCors();

var app = builder.Build();

app.MapHub<TemperatureHub>("/temperatureHub");

app.RegisterSensorEndpoints();
app.UseSwagger();
app.UseSwaggerUI();

// Very non-recommended CORS settings.
// We need to add it to make SignalR work.
// Please do not use in production.
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.Run();