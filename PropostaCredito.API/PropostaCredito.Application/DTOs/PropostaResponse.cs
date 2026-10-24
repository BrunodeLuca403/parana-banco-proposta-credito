using System;
using PropostaCredito.Core.Entitys;
using PropostaCredito.Core.Enum;

namespace PropostaCredito.Application.DTOs
{
    public class PropostaResponse
    {
        public Guid Id { get; set; }
        public Guid IdCliente { get; set; }
        public int Score { get; set; }
        public StatusProposta Status { get; set; }
        public decimal LimitePorCartao { get; set; }
        public int QuantidadeCartoes { get; set; }
        public DateTime CriadoEm { get; set; }


        public static PropostaResponse FromEntity(Proposta proposta) => new()
        {
                Id = proposta.Id,
                IdCliente = proposta.IdCliente,
                Score = proposta.Score,
                Status = proposta.Status,
                LimitePorCartao = proposta.LimitePorCartao,
                QuantidadeCartoes = proposta.QuantidadeCartoes,
                CriadoEm = proposta.CreatAt
        };
    }
}
