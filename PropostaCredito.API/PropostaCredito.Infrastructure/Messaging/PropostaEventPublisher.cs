using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PropostaCredito.Core.Events;
using RabbitMQ.Client;

namespace PropostaCredito.Infrastructure.Messaging
{
    public class PropostaEventPublisher : IPropostaEventPublisher
    {
        private readonly IRabbitMqConnection _connection;
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<PropostaEventPublisher> _logger;

        public PropostaEventPublisher(
            IRabbitMqConnection connection,
            IOptions<RabbitMqSettings> options,
            ILogger<PropostaEventPublisher> logger)
        {
            _connection = connection;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task PublicarPropostaGeradaAsync(PropostaGeradaEvent evento, CancellationToken cancellationToken = default)
        {
            await using var channel = await _connection.CreateChannelAsync(cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: _settings.PublishExchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var json = JsonSerializer.Serialize(evento);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString(),
                Type = nameof(PropostaGeradaEvent)
            };

            await channel.BasicPublishAsync(
                exchange: _settings.PublishExchange,
                routingKey: _settings.PropostaGeradaRoutingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Evento PropostaGerada publicado. ClienteId={ClienteId}, Exchange={Exchange}, RoutingKey={RoutingKey}",
                evento.ClienteId, _settings.PublishExchange, _settings.PropostaGeradaRoutingKey);
        }
    }
}
