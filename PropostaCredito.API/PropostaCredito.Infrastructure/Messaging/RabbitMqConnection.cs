using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace PropostaCredito.Infrastructure.Messaging
{
    public class RabbitMqConnection : IRabbitMqConnection
    {
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqConnection> _logger;
        private readonly ConnectionFactory _factory;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private IConnection? _connection;

        public RabbitMqConnection(IOptions<RabbitMqSettings> options, ILogger<RabbitMqConnection> logger)
        {
            _settings = options.Value;
            _logger = logger;

            _factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
        }

        public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (_connection is { IsOpen: true })
                    return _connection;

                _logger.LogInformation(
                    "Abrindo conexão com RabbitMQ em {Host}:{Port}", _settings.HostName, _settings.Port);

                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
        {
            var connection = await GetConnectionAsync(cancellationToken);
            return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }

            _connectionLock.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
