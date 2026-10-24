using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<PropostaResponse>> ObterPropostaCliente([FromRoute] Guid id)
        {
            var proposta = await _propostaService.ObterPropostaClienteAsync(id);
            if (!proposta.IsSuccess)
            {
                return NotFound(proposta.Error);
            }
            return Ok(proposta.Value);
        }
    }
}
