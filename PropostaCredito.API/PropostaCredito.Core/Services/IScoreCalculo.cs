using System;
using System.Collections.Generic;
using System.Text;

namespace PropostaCredito.Core.Services
{
    public interface IScoreCalculo
    {
        int Calculo(decimal rendaMensal, DateTime dataNascimento);

    }
}
