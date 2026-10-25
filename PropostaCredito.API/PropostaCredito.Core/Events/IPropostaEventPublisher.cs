namespace PropostaCredito.Core.Events
{
    public interface IPropostaEventPublisher
    {
        Task PublicarPropostaGeradaAsync(PropostaGeradaEvent evento, CancellationToken cancellationToken = default);
    }
}
