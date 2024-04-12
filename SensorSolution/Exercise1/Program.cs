using Exercise1.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.RegisterSensorEndpoints();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();