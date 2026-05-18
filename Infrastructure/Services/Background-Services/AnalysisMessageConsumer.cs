using System.Text;
using System.Text.Json;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Infrastructure.Interfaces;
using Infrastructure.Models.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace Infrastructure.Services.Background_Services;

public class AnalysisMessageConsumer : BackgroundService
{
    private readonly ILogger<AnalysisMessageConsumer> _logger;
    private readonly ConnectionFactory _factory;

    private readonly string _responseQueue;
    private readonly string _exchangeName;
    private readonly string _routingKey;

    private IConnection? _connection;
    private IChannel? _channel;
    private readonly IServiceScopeFactory _scopeFactory;

    public AnalysisMessageConsumer(IConfiguration configuration, ILogger<AnalysisMessageConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        // Match the Python config keys
        _responseQueue = configuration["RabbitMQ:analysis_response_queue"] ?? "response_queue";
        _exchangeName = configuration["RabbitMQ:exchange_name"] ?? "aml_exchange";
        _routingKey = configuration["RabbitMQ:analysis_response_routing_key"] ?? "response_routing_key";

        _factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:host_name"],
            Port = int.Parse(configuration["RabbitMQ:port"] ?? "5672"),
            UserName = configuration["RabbitMQ:user_name"],
            Password = configuration["RabbitMQ:password"]
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting RabbitMQ Consumer Service...");

        try
        {
            // 1. Establish Async Connection
            _connection = await _factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            // 2. Mirror the Python Topology
            await _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: ExchangeType.Direct,
                cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: _responseQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: _responseQueue,
                exchange: _exchangeName,
                routingKey: _routingKey,
                cancellationToken: stoppingToken);

            // 3. Set QoS (Prefetch Count) to 1 so the worker processes one message at a time
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false,
                cancellationToken: stoppingToken);

            // 4. Setup the Async Consumer
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var messageStr = Encoding.UTF8.GetString(body);
                var correlationId = ea.BasicProperties.CorrelationId;

                try
                {
                    _logger.LogInformation("Received processing result for CorrelationId: {Id}", correlationId);

                    // 1. Deserialize the JSON into our C# class
                    var result = JsonSerializer.Deserialize<AnalysisResponseMessage>(messageStr);
                    // Changes "&& result.Success"
                    if (result is not null && result.Success)
                    {
                        var scope = _scopeFactory.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService<IResponseMessageService<AnalysisResponseMessage>>();
                        var repo = scope.ServiceProvider.GetRequiredService<IAnalysisRepository>();
                        // Deserialize and Insert Data into Database then update status
                        await service.DoWork(result, default);
                    }
                    // Acknowledge the message upon successful processing
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false,
                        cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message [CorrelationId: {Id}]", correlationId);
                    await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true,
                        cancellationToken: stoppingToken);
                }
            };

            // 7. Start consuming (autoAck is false so we can manually ack/nack)
            await _channel.BasicConsumeAsync(
                queue: _responseQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("Listening for messages on {Queue}...", _responseQueue);

            // 8. Keep the BackgroundService alive until the application is shutting down
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMQ Consumer Service is stopping due to app shutdown.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "A fatal error occurred in the RabbitMQ Consumer Service.");
        }
        finally
        {
            await CleanUpAsync();
        }
    }

    private async Task CleanUpAsync()
    {
        if (_channel is not null && _channel.IsOpen)
        {
            await _channel.CloseAsync();
        }

        if (_connection is not null && _connection.IsOpen)
        {
            await _connection.CloseAsync();
        }

        _logger.LogInformation("RabbitMQ Consumer connection closed.");
    }
}