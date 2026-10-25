using PropostaCredito.Core.Services;

namespace PropostaCredito.Tests
{
    // Cálculo de score = pontos por renda + pontos por idade (resultado entre 0 e 1000).
    //
    // Pontos por renda:  <=1500:50  <=3000:200  <=6000:400  <=10000:600  >10000:800
    // Pontos por idade:  <22:50     <30:100     <60:200     >=60:120
    public class ScoreCalculoTests
    {
        private readonly ScoreCalculo _sut = new();

        // Idade fixada em ~40 anos (faixa <60 => 200 pontos de idade) para isolar
        // a variação pela renda.
        private static DateTime Nascimento40Anos => DateTime.Today.AddYears(-40);

        [Theory]
        [InlineData(1000, 250)]   // renda<=1500:50  + idade 200 = 250
        [InlineData(2500, 400)]   // renda<=3000:200 + idade 200 = 400
        [InlineData(5000, 600)]   // renda<=6000:400 + idade 200 = 600
        [InlineData(9000, 800)]   // renda<=10000:600+ idade 200 = 800
        [InlineData(15000, 1000)] // renda>10000:800 + idade 200 = 1000
        public void Calculo_VariandoRenda_DeveSomarPontosCorretos(decimal renda, int esperado)
        {
            var score = _sut.Calculo(renda, Nascimento40Anos);

            Assert.Equal(esperado, score);
        }

        [Fact]
        public void Calculo_JovemComRendaBaixa_DeveGerarScoreBaixo()
        {
            // 20 anos (idade <22 => 50) + renda 1000 (<=1500 => 50) = 100 -> faixa negada.
            var nascimento = DateTime.Today.AddYears(-20);

            var score = _sut.Calculo(1000m, nascimento);

            Assert.Equal(100, score);
        }

        [Fact]
        public void Calculo_NuncaUltrapassa1000()
        {
            // Renda altíssima + idade de maior pontuação: resultado é limitado a 1000.
            var score = _sut.Calculo(1_000_000m, DateTime.Today.AddYears(-40));

            Assert.True(score <= 1000);
        }
    }
}
