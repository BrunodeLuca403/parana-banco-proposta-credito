namespace PropostaCredito.Core.Events
{
    /// <summary>
    /// Evento de integração publicado quando uma proposta é gerada.
    /// Contrato consumido pelo serviço de Cartão. Carrega o resultado do score
    /// (fat event) para o Cartão emitir os cartões sem consultar a Proposta de volta.
    /// </summary>
    public record PropostaGeradaEvent(
        Guid PropostaId,
        Guid ClienteId,
        int Score,
        bool Aprovada,
        decimal LimitePorCartao,
        int QuantidadeCartoes,
        DateTime OcorridoEm);
}
