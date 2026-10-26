using Microsoft.AspNetCore.Mvc;
using PropostaCredito.Application.Common;
using PropostaCredito.Application.DTOs;
using PropostaCredito.Application.Services.Interface;

namespace PropostaCredito.API.Controllers
{
    [ApiController]
    [Route("/api/v1/propostas/")]
    public class PropostasController : ControllerBase
    {
        private readonly IPropostaService _propostaService;

        public PropostasController(IPropostaService propostaService)
        {
            _propostaService = propostaService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ResultViewModel<PropostaResponse?>>> ObterPropostaCliente([FromRoute] Guid id)
        {
            var proposta = await _propostaService.ObterPropostaClienteAsync(id);

            if (!proposta.IsSuccess)
            {
                return ResultViewModel<PropostaResponse?>.Failure(
                  Error.NotFound(
                      "PropostaCredito.API.Controllers.ObterPropostaCliente",
                      $"Cliente com Id {proposta} não encontrado"));
            }
           

            return ResultViewModel<PropostaResponse?>.Success(proposta.Value);
        }
    }
}
