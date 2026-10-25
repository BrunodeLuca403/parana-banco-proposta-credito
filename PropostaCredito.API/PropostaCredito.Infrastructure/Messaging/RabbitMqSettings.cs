namespace PropostaCredito.Infrastructure.Messaging
{
    public class RabbitMqSettings
    {
        public const string SectionName = "RabbitMq";
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string Exchange { get; set; } = "clientes.exchange";
        public string ClienteCadastradoRoutingKey { get; set; } = "cliente.cadastrado";
        public string Queue { get; set; } = "proposta.cliente-cadastrado";
        public string DeadLetterExchange { get; set; } = "proposta.dlx";
        public string DeadLetterRoutingKey { get; set; } = "proposta.cliente-cadastrado.dead";
        public string DeadLetterQueue { get; set; } = "proposta.cliente-cadastrado.dlq";
        public string PublishExchange { get; set; } = "proposta.exchange";
        public string PropostaGeradaRoutingKey { get; set; } = "proposta.gerada";
        public int MaxRetryAttempts { get; set; } = 5;
        public int RetryBaseDelaySeconds { get; set; } = 2;
    }
}
