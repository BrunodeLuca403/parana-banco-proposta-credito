using RabbitMQ.Client;

namespace PropostaCredito.Infrastructure.Messaging
{
    public interface IRabbitMqConnection : IAsyncDisposable
    {
        Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
        Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default);
    }
}
