using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    try
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "weather_logs", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { Event = "WeatherRequested", Time = DateTime.Now }));

        await channel.BasicPublishAsync(exchange: "", routingKey: "weather_logs", body: body);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Rabbit Error: {ex.Message}");
    }

    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}