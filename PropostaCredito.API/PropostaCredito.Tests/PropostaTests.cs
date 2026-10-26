using PropostaCredito.Core.Entitys;
using PropostaCredito.Core.Enum;

namespace PropostaCredito.Tests
{

    public class PropostaTests
    {
        private static readonly Guid ClienteId = Guid.NewGuid();

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(100)]
        public void Criar_QuandoScoreAte100_DeveNegarSemCartoes(int score)
        {
            var proposta = Proposta.Criar(ClienteId, score);

            Assert.Equal(StatusProposta.Negado, proposta.Status);
            Assert.Equal(0m, proposta.LimitePorCartao);
            Assert.Equal(0, proposta.QuantidadeCartoes);
        }

        [Theory]
        [InlineData(101)]
        [InlineData(300)]
        [InlineData(500)]
        public void Criar_QuandoScoreEntre101E500_DeveAprovarComUmCartaoDe1000(int score)
        {
            var proposta = Proposta.Criar(ClienteId, score);

            Assert.Equal(StatusProposta.Aprovado, proposta.Status);
            Assert.Equal(1000m, proposta.LimitePorCartao);
            Assert.Equal(1, proposta.QuantidadeCartoes);
        }

        [Theory]
        [InlineData(501)]
        [InlineData(750)]
        [InlineData(1000)]
        public void Criar_QuandoScoreEntre501E1000_DeveAprovarComDoisCartoesDe5000(int score)
        {
            var proposta = Proposta.Criar(ClienteId, score);

            Assert.Equal(StatusProposta.Aprovado, proposta.Status);
            Assert.Equal(5000m, proposta.LimitePorCartao);
            Assert.Equal(2, proposta.QuantidadeCartoes);
        }

        [Fact]
        public void Criar_QuandoIdClienteVazio_DeveLancarArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Proposta.Criar(Guid.Empty, 500));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1001)]
        public void Criar_QuandoScoreForaDoIntervalo_DeveLancarArgumentOutOfRange(int score)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Proposta.Criar(ClienteId, score));
        }
    }
}
