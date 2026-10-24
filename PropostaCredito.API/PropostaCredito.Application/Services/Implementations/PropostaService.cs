using PropostaCredito.Application.Common;
using PropostaCredito.Application.DTOs;
using PropostaCredito.Application.Services.Interface;
using PropostaCredito.Core.Entitys;
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

        public PropostaService(IPropostaRepository propostaRepository, IScoreCalculo scoreCalculo)
        {
            _propostaRepository = propostaRepository;
            _scoreCalculo = scoreCalculo;
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

            var response = MapearParaResponse(proposta);
            return ResultViewModel<PropostaResponse>.Success(response);
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
