using PropostaCredito.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropostaCredito.Core.Entitys
{
    public class Proposta : BaseEntity
    {
        public Proposta(Guid idCliente, int score, StatusProposta status, decimal limitePorCartao, int quantidadeCartoes)
        {
            IdCliente = idCliente;
            Score = score;
            Status = status;
            LimitePorCartao = limitePorCartao;
            QuantidadeCartoes = quantidadeCartoes;
        }

        public Guid IdCliente { get; private set; }
        public int Score { get; private set; }
        public StatusProposta Status { get; private set; }
        public decimal LimitePorCartao { get; private set; }
        public int QuantidadeCartoes { get; private set; }
    }
}
