namespace Application.Interfaces.Services;

public interface IMessageProducer<in T>
{
    Task<int> PublishAsync(T message,CancellationToken ct);
}