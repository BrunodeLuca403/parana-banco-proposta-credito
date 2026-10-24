using System;

namespace PropostaCredito.Application.DTOs
{
    public class GerarPropostaRequest
    {
        public Guid IdCliente { get; set; }
        public decimal RendaMensal { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
