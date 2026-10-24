using System;
using System.Collections.Generic;
using System.Text;

namespace PropostaCredito.Core.Services
{
    public class ScoreCalculo : IScoreCalculo
    {
        public int Calculo(decimal rendaMensal, DateTime dataNascimento)
        {
            var score = PontosPorRenda(rendaMensal) + PontosPorIdade(dataNascimento);
            return Math.Clamp(score, 0, 1000);
        }

        private static int PontosPorRenda(decimal renda) => renda switch
        {
            <= 1500m => 50,
            <= 3000m => 200,
            <= 6000m => 400,
            <= 10000m => 600,
            _ => 800
        };
        private static int PontosPorIdade(DateTime nascimento)
        {
            var idade = CalcularIdade(nascimento);
            return idade switch
            {
                < 22 => 50,
                < 30 => 100,
                < 60 => 200,
                _ => 120
            };
        }

        private static int CalcularIdade(DateTime nascimento)
        {
            var hoje = DateTime.Today;
            var idade = hoje.Year - nascimento.Year;
            if (nascimento.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }

    }
}
