using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PropostaCredito.Application.Services.Implementations;
using PropostaCredito.Core.Entitys;
using PropostaCredito.Core.Events;
using PropostaCredito.Core.Repository;
using PropostaCredito.Core.Services;
using PropostaCredito.Tests.Helpers;

namespace PropostaCredito.Tests
{
    public class PropostaServiceTests
    {
        private readonly Mock<IPropostaRepository> _repositoryMock = new();
        private readonly Mock<IScoreCalculo> _scoreMock = new();
        private readonly Mock<IPropostaEventPublisher> _publisherMock = new();
        private readonly PropostaService _sut;

        public PropostaServiceTests()
        {
            _sut = new PropostaService(
                _repositoryMock.Object,
                _scoreMock.Object,
                _publisherMock.Object,
                NullLogger<PropostaService>.Instance);
        }

        [Fact]
        public async Task GerarProposta_QuandoNaoExiste_DeveGerarEPublicarEvento()
        {
            // Arrange — não existe proposta e o score cai na faixa 501-1000 (2 cartões).
            var request = FakerPropostaHelper.CriarRequest();
            _repositoryMock.Setup(r => r.BuscarProposta(request.IdCliente)).ReturnsAsync((Proposta?)null);
            _scoreMock.Setup(s => s.Calculo(request.RendaMensal, request.DataNascimento)).Returns(700);

            // Act
            var resultado = await _sut.GerarProposta(request);

            // Assert
            Assert.True(resultado.IsSuccess);
            _repositoryMock.Verify(r => r.GerarProposta(It.IsAny<Proposta>()), Times.Once);

            // Publica o evento com o resultado correto (aprovada, 2 cartões de 5000).
            _publisherMock.Verify(p => p.PublicarPropostaGeradaAsync(
                It.Is<PropostaGeradaEvent>(e =>
                    e.ClienteId == request.IdCliente &&
                    e.Aprovada &&
                    e.QuantidadeCartoes == 2 &&
                    e.LimitePorCartao == 5000m),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GerarProposta_QuandoScoreNegado_DevePublicarEventoNaoAprovado()
        {
            // Arrange — score na faixa 0-100 (negado).
            var request = FakerPropostaHelper.CriarRequest();
            _repositoryMock.Setup(r => r.BuscarProposta(request.IdCliente)).ReturnsAsync((Proposta?)null);
            _scoreMock.Setup(s => s.Calculo(It.IsAny<decimal>(), It.IsAny<DateTime>())).Returns(50);

            // Act
            await _sut.GerarProposta(request);

            // Assert — o evento é publicado mesmo quando negado (Aprovada = false).
            _publisherMock.Verify(p => p.PublicarPropostaGeradaAsync(
                It.Is<PropostaGeradaEvent>(e => !e.Aprovada && e.QuantidadeCartoes == 0),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GerarProposta_QuandoJaExiste_DeveFalharENaoPublicar()
        {
            // Arrange — já existe uma proposta para o cliente.
            var request = FakerPropostaHelper.CriarRequest();
            var existente = Proposta.Criar(request.IdCliente, 500);
            _repositoryMock.Setup(r => r.BuscarProposta(request.IdCliente)).ReturnsAsync(existente);

            // Act
            var resultado = await _sut.GerarProposta(request);

            // Assert
            Assert.False(resultado.IsSuccess);
            _repositoryMock.Verify(r => r.GerarProposta(It.IsAny<Proposta>()), Times.Never);
            _publisherMock.Verify(p => p.PublicarPropostaGeradaAsync(
                It.IsAny<PropostaGeradaEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ProcessarClienteCadastrado_QuandoPropostaJaExiste_NaoDeveLancar()
        {
            // Arrange — idempotência: evento duplicado (proposta já existe) não é erro.
            var evento = FakerPropostaHelper.CriarClienteCadastradoEvent();
            var existente = Proposta.Criar(evento.ClienteId, 500);
            _repositoryMock.Setup(r => r.BuscarProposta(evento.ClienteId)).ReturnsAsync(existente);

            // Act
            var excecao = await Record.ExceptionAsync(() => _sut.ProcessarClienteCadastradoAsync(evento));
            Assert.Null(excecao);
        }
    }
}
