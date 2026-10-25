using PropostaCredito.Core.Enum;
using System;

namespace PropostaCredito.Core.Entitys
{
    public class Proposta : BaseEntity
    {
        private Proposta(Guid idCliente, int score, StatusProposta status, decimal limitePorCartao, int quantidadeCartoes)
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

        public static Proposta Criar(Guid idCliente, int score)
        {
            if (idCliente == Guid.Empty)
                throw new ArgumentException("IdCliente é obrigatório.", nameof(idCliente));

            if (score is < 0 or > 1000)
                throw new ArgumentOutOfRangeException(nameof(score), score, "Score deve estar entre 0 e 1000.");

            var (status, limitePorCartao, quantidadeCartoes) = score switch
            {

                // 0 – 100: Negado, sem limite e sem cartões.
                <= 100 => (StatusProposta.Negado, 0m, 0),

                // 101 – 500: Aprovado, R$ 1.000 por cartão, 1 cartão.
                <= 500 => (StatusProposta.Aprovado, 1000m, 1),
                _ => (StatusProposta.Aprovado, 5000m, 2)
            };

            return new Proposta(idCliente, score, status, limitePorCartao, quantidadeCartoes);
        }
    }
}
