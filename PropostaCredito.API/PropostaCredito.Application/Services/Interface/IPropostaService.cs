using PropostaCredito.Application.Common;
using PropostaCredito.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropostaCredito.Application.Services.Interface
{
    public interface IPropostaService
    {
        Task<ResultViewModel<PropostaResponse>> GerarProposta(GerarPropostaRequest request);
        Task<ResultViewModel<PropostaResponse?>> ObterPropostaClienteAsync(Guid clienteId);

    }
}
