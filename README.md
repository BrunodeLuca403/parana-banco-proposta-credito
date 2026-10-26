# Paraná Banco — Proposta de Crédito

Microsserviço que consome o evento `ClienteCadastrado`, calcula o score de crédito,
gera a proposta e publica o evento `PropostaGerada`. É o elo central do fluxo: atua
como **consumidor** (do Cliente) e **produtor** (para o Cartão).

## Arquitetura

```
PropostaCredito.API            → Controllers, Program.cs, configuração
PropostaCredito.Application    → Services (geração/processamento), DTOs, Common
PropostaCredito.Core           → Entidade Proposta, cálculo de score, eventos, publisher
PropostaCredito.Infrastructure → EF Core (SQL Server), repositório, mensageria (consumer + publisher)
PropostaCredito.Tests          → Testes de unidade (xUnit + Moq + Bogus)
```

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/v1/propostas/{clienteId}` | Consulta a proposta de um cliente |

A proposta não é criada via REST — ela é gerada automaticamente ao consumir o evento
`ClienteCadastrado`.

## Regras de score

O score (0–1000) é calculado a partir da renda mensal e da idade. A faixa determina
o resultado:

| Score | Status | Limite por cartão | Qtd. cartões |
|-------|--------|-------------------|--------------|
| 0 – 100 | Negado | — | 0 |
| 101 – 500 | Aprovado | R$ 1.000,00 | 1 |
| 501 – 1000 | Aprovado | R$ 5.000,00 | até 2 |

## Mensageria

**Consome** `ClienteCadastrado`:
- Fila: `proposta.cliente-cadastrado`, ligada à exchange `clientes.exchange`
  (routing key `cliente.cadastrado`).

**Publica** `PropostaGerada` (com o resultado do score — inclusive quando negada):
- Exchange: `proposta.exchange` (topic, durable)
- Routing key: `proposta.gerada`

### Resiliência

- **Retry com backoff exponencial**: até 5 tentativas (2s, 4s, 8s, 16s) para falhas
  transitórias.
- **Dead-letter queue**: mensagens que esgotam as tentativas, ou com payload inválido
  (erro permanente), são desviadas para `proposta.cliente-cadastrado.dlq` via a
  dead-letter exchange `proposta.dlx` — sem travar a fila principal nem perder a mensagem.
- **Idempotência**: se a proposta já existe (entrega duplicada, *at-least-once*), o
  evento é tratado como sucesso e confirmado (ACK), sem reprocessar.

## Variáveis de ambiente / configuração

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=ParanaBancoProposta;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Exchange": "clientes.exchange",
    "ClienteCadastradoRoutingKey": "cliente.cadastrado",
    "Queue": "proposta.cliente-cadastrado",
    "DeadLetterExchange": "proposta.dlx",
    "DeadLetterRoutingKey": "proposta.cliente-cadastrado.dead",
    "DeadLetterQueue": "proposta.cliente-cadastrado.dlq",
    "PublishExchange": "proposta.exchange",
    "PropostaGeradaRoutingKey": "proposta.gerada",
    "MaxRetryAttempts": 5,
    "RetryBaseDelaySeconds": 2
  }
}
```

## Como executar localmente

Pré-requisitos: .NET 10 SDK, SQL Server, RabbitMQ (ver README do serviço de Clientes
para subir o RabbitMQ via Docker).

1. **Aplicar as migrations:**
   ```bash
   dotnet ef database update --project PropostaCredito.Infrastructure --startup-project PropostaCredito.API
   ```

2. **Rodar a API:**
   ```bash
   dotnet run --project PropostaCredito.API
   ```
   Disponível em `http://localhost:5176`. O consumer inicia automaticamente e passa a
   escutar a fila.

## Testes

```bash
dotnet test
```

Cobrem: as regras de score (`Proposta.Criar`) com foco em valores de fronteira, o
cálculo de score (`ScoreCalculo`), e o `PropostaService` (geração, publicação do
evento e idempotência).

## Decisões técnicas

- **Retry na aplicação com backoff + DLQ**: distingue erro transitório (vale retry)
  de permanente (JSON inválido vai direto para a DLQ, sem gastar tentativas).
- **Consumer como BackgroundService**: roda continuamente; cria um *scope* por mensagem
  (`IServiceScopeFactory`) para evitar *captive dependency* entre o consumer (singleton)
  e o `DbContext` (scoped).
- **PropostaGerada publicado sempre**: inclusive quando negada. O campo `Aprovada`
  carrega o resultado e o serviço de Cartão decide o que fazer.
- **O produtor não conhece o consumidor**: publica na `proposta.exchange`; a fila e o
  binding são de responsabilidade do serviço de Cartão, preservando o desacoplamento.
```
