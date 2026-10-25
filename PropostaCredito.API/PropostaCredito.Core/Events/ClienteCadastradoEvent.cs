namespace PropostaCredito.Core.Events
{
    public record ClienteCadastradoEvent(
        Guid ClienteId,
        string Nome,
        string Cpf,
        string Email,
        decimal RendaMensal,
        DateTime DataNascimento,
        DateTime OcorridoEm);
}
