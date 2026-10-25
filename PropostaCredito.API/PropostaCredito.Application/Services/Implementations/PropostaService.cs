using Microsoft.Extensions.Logging;
using PropostaCredito.Application.Common;
using PropostaCredito.Application.DTOs;
using PropostaCredito.Application.Services.Interface;
using PropostaCredito.Core.Entitys;
using PropostaCredito.Core.Enum;
using PropostaCredito.Core.Events;
using PropostaCredito.Core.Repository;
using PropostaCredito.Core.Services;
using System;
using System.Threading.Tasks;

namespace PropostaCredito.Application.Services.Implementations
{
    public class PropostaService : IPropostaService
    {
        private readonly IPropostaRepository _propostaRepository;
        private readonly IScoreCalculo _scoreCalculo;
        private readonly IPropostaEventPublisher _eventPublisher;
        private readonly ILogger<PropostaService> _logger;

        public PropostaService(
            IPropostaRepository propostaRepository,
            IScoreCalculo scoreCalculo,
            IPropostaEventPublisher eventPublisher,
            ILogger<PropostaService> logger)
        {
            _propostaRepository = propostaRepository;
            _scoreCalculo = scoreCalculo;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<ResultViewModel<PropostaResponse>> GerarProposta(GerarPropostaRequest request)
        {
            var propostaExistente = await _propostaRepository.BuscarProposta(request.IdCliente);
            if (propostaExistente is not null)
            {
                return ResultViewModel<PropostaResponse>.Failure(
                    Error.Conflict(
                        "PropostaCredito.Application.Services.Implementations.GerarProposta",
                        $"Já existe uma proposta para o cliente com Id {request.IdCliente}"));
            }

            var score = _scoreCalculo.Calculo(request.RendaMensal, request.DataNascimento);

            var proposta = Proposta.Criar(request.IdCliente, score);

            await _propostaRepository.GerarProposta(proposta);

            var evento = new PropostaGeradaEvent(
                proposta.Id,
                proposta.IdCliente,
                proposta.Score,
                proposta.Status == StatusProposta.Aprovado,
                proposta.LimitePorCartao,
                proposta.QuantidadeCartoes,
                DateTime.UtcNow);

            await _eventPublisher.PublicarPropostaGeradaAsync(evento);

            var response = MapearParaResponse(proposta);
            return ResultViewModel<PropostaResponse>.Success(response);
        }

        public async Task ProcessarClienteCadastradoAsync(ClienteCadastradoEvent evento, CancellationToken cancellationToken = default)
        {
            var request = new GerarPropostaRequest
            {
                IdCliente = evento.ClienteId,
                RendaMensal = evento.RendaMensal,
                DataNascimento = evento.DataNascimento
            };

            var resultado = await GerarProposta(request);

            if (resultado.IsSuccess)
            {
                _logger.LogInformation("Proposta gerada para ClienteId={ClienteId}", evento.ClienteId);
                return;
            }

            if (resultado.Error.Type == ErrorTypeEnum.Conflict)
            {
                _logger.LogInformation(
                    "Proposta já existia para ClienteId={ClienteId} (evento duplicado ignorado).",
                    evento.ClienteId);
                return;
            }

            throw new InvalidOperationException(
                $"Falha ao gerar proposta para ClienteId={evento.ClienteId}: {resultado.Error.Description}");
        }

        public async Task<ResultViewModel<PropostaResponse?>> ObterPropostaClienteAsync(Guid clienteId)
        {
            var propostaCliente = await _propostaRepository.BuscarProposta(clienteId);

            if (propostaCliente is null)
            {
                return ResultViewModel<PropostaResponse?>.Failure(
                    Error.NotFound(
                        "PropostaCredito.Application.Services.Implementations.ObterPropostaClienteAsync",
                        $"Cliente com Id {clienteId} não encontrado"));
            }

            var dtoProposta = MapearParaResponse(propostaCliente);
            return ResultViewModel<PropostaResponse?>.Success(dtoProposta);
        }

        private static PropostaResponse MapearParaResponse(Proposta proposta) => new()
        {
            Id = proposta.Id,
            IdCliente = proposta.IdCliente,
            Score = proposta.Score,
            Status = proposta.Status,
            LimitePorCartao = proposta.LimitePorCartao,
            QuantidadeCartoes = proposta.QuantidadeCartoes,
            CriadoEm = proposta.CreatAt
        };
    }
}
