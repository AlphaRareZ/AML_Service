using System.Text;
using System.Text.Json;
using Application.Interfaces.Services;
using Application.UseCases.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.Services.MessageBus;

// 1. Switched from IDisposable to IAsyncDisposable
public class AnalysisMessageProducer(IConfiguration configuration, ILogger<AnalysisMessageProducer> logger)
    : IMessageProducer<AnalysisMessage>, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel; // 2. IModel is now IChannel
    
    private readonly ConnectionFactory _factory = new()
    {
        HostName = configuration["RabbitMQ:host_name"],
        Port = int.Parse(configuration["RabbitMQ:port"] ?? "5672"),
        UserName = configuration["RabbitMQ:user_name"],
        Password = configuration["RabbitMQ:password"]
    };

    private readonly string _requestQueue = configuration["RabbitMQ:analysis_request_queue"] ?? "request_queue";
    
    // 3. Use SemaphoreSlim to safely handle async locking
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    // Setup factory, but DO NOT connect yet. Constructors must be fast and synchronous.

    // 4. Helper method to ensure we are connected before publishing
    private async Task EnsureConnectionAsync(CancellationToken ct)
    {
        // If already open, skip
        if (_channel is { IsOpen: true }) return;

        await _connectionLock.WaitAsync(ct);
        try
        {
            // Double-check inside the lock
            if (_channel is { IsOpen: true }) return;

            _connection = await _factory.CreateConnectionAsync(ct);
            _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
            
            await _channel.QueueDeclareAsync(
                queue: _requestQueue, 
                durable: true, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null,
                cancellationToken: ct);
                
            logger.LogInformation("Connected to RabbitMQ and verified queue: {Queue}", _requestQueue);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<int> PublishAsync(AnalysisMessage message, CancellationToken ct)
    {
        // Ensure connection is established (will only run the connection logic once)
        await EnsureConnectionAsync(ct);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        // 5. BasicProperties is now a simple class you instantiate
        var properties = new BasicProperties
        {
            Persistent = true, 
            ContentType = "application/json" 
        };

        // 6. BasicPublishAsync is natively thread-safe in v7, no lock needed!
        await _channel!.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _requestQueue,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: ct);

        // 7. Get the queue count asynchronously
        var queueInfo = await _channel.QueueDeclarePassiveAsync(_requestQueue, cancellationToken: ct);
        var currentMessageCount = (int)queueInfo.MessageCount;

        logger.LogInformation(
            "Published message to {Queue}. Messages currently in queue: {Count}", 
            _requestQueue, 
            currentMessageCount);

        return currentMessageCount;
    }

    public async ValueTask DisposeAsync()
    {
        // 8. Properly dispose of async resources
        if (_channel is not null) await _channel.CloseAsync();
        if (_connection is not null) await _connection.CloseAsync();
        
        _connectionLock.Dispose();
        logger.LogInformation("RabbitMQ producer connection closed.");
    }

    
}