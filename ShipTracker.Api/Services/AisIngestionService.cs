using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ShipTracker.Api.Models;
using ShipTracker.Api.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ShipTracker.Api.Services;

public class AisIngestionService : BackgroundService
{
    private readonly ILogger<AisIngestionService> _logger;
    private const string AisStreamUrl = "wss://stream.aisstream.io/v0/stream";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _apiKey;

    public AisIngestionService(ILogger<AisIngestionService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _apiKey = configuration["AisStream:ApiKey"]
            ?? throw new InvalidOperationException("Missing configuration value 'AisStream:ApiKey'. Set it via user-secrets locally or an AisStream__ApiKey environment variable in production.");
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Outer loop: keep reconnecting if the connection drops or something throws.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal shutdown (app stopping) - not an error.
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AIS ingestion loop crashed unexpectedly. Reconnecting in 5 seconds.");
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task RunOnceAsync(CancellationToken stoppingToken)
    {
        using var client = new ClientWebSocket();

        await client.ConnectAsync(new Uri(AisStreamUrl), stoppingToken);
        _logger.LogInformation("Connected to AISStream.");

        var subscription = new
        {
            APIKey = _apiKey,
            BoundingBoxes = new[] { new[] { new[] { 5.0, 60.0 }, new[] { 25.0, 95.0 } } },
            FilterMessageTypes = new[] { "PositionReport", "ShipStaticData" }
        };

        var subscriptionJson = JsonSerializer.Serialize(subscription);
        var subscriptionBytes = Encoding.UTF8.GetBytes(subscriptionJson);
        await client.SendAsync(subscriptionBytes, WebSocketMessageType.Text, true, stoppingToken);
        _logger.LogInformation("Subscription sent.");

        var buffer = new byte[8192];

        while (!stoppingToken.IsCancellationRequested && client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(buffer, stoppingToken);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                _logger.LogWarning("AISStream closed the connection. Status: {Status}, Description: {Description}",
                    result.CloseStatus, result.CloseStatusDescription);
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            AisMessage? aisMessage = JsonSerializer.Deserialize<AisMessage>(message, options);

            if (aisMessage is null || aisMessage.Message is null)
            {
                _logger.LogWarning("Received null or malformed AIS message: {Raw}", message);
                continue;
            }

            PositionReport? positionReport = aisMessage.Message.PositionReport;


            if (positionReport is null)
            {
                // Log the raw message and the deserialized object for investigation, then skip processing
                _logger.LogWarning("Received AIS message without PositionReport. Raw: {Raw}; Deserialized: {@AisMessage}", message, aisMessage);
                continue;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var mmsi = aisMessage.MetaData?.MMSI.ToString() ?? positionReport.UserID.ToString();
            var name = aisMessage.MetaData?.ShipName?.Trim();

            var existing = await db.Ships.FirstOrDefaultAsync(s => s.Mmsi == mmsi, stoppingToken);
            if (existing is null)
            {
                db.Ships.Add(new Ship
                {
                    Mmsi = mmsi,
                    Name = string.IsNullOrWhiteSpace(name) ? null : name,
                    Latitude = positionReport.Latitude,
                    Longitude = positionReport.Longitude,
                    SpeedOverGround = positionReport.Sog,
                    CourseOverGround = positionReport.Cog,
                    Speed = positionReport.Sog,
                    Heading = positionReport.TrueHeading,
                    LastUpdatedUtc = DateTime.UtcNow
                });
            }
            else
            {
                existing.Latitude = positionReport.Latitude;
                existing.Longitude = positionReport.Longitude;
                existing.SpeedOverGround = positionReport.Sog;
                existing.CourseOverGround = positionReport.Cog;
                existing.Speed = positionReport.Sog;
                existing.Heading = positionReport.TrueHeading;
                if (!string.IsNullOrWhiteSpace(name)) existing.Name = name;
                existing.LastUpdatedUtc = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Received: {Message}", positionReport);
        }
    }
}