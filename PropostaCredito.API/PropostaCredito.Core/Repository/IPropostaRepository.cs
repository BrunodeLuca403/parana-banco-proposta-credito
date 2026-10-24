using PropostaCredito.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropostaCredito.Core.Repository
{
    public interface IPropostaRepository
    {
        Task GerarProposta(Proposta proposta);
        Task<Proposta?> BuscarProposta(Guid ClientId);
    }
}
