using Infrastructure.Models.MessageBus;

namespace Infrastructure.Interfaces;

public interface IResponseMessageService<T>
{
    Task DoWork(T? response,CancellationToken token);
}