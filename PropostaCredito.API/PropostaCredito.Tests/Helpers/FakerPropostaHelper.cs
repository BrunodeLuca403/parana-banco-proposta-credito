using Bogus;
using Bogus.Extensions.Brazil;
using PropostaCredito.Application.DTOs;
using PropostaCredito.Core.Events;

namespace PropostaCredito.Tests.Helpers
{
    public static class FakerPropostaHelper
    {
        private static readonly Faker<GerarPropostaRequest> _requestFaker =
            new Faker<GerarPropostaRequest>("pt_BR")
                .RuleFor(r => r.IdCliente, f => f.Random.Guid())
                .RuleFor(r => r.RendaMensal, f => f.Finance.Amount(1000, 20000))
                .RuleFor(r => r.DataNascimento, f => f.Date.Between(
                    DateTime.Today.AddYears(-80), DateTime.Today.AddYears(-18)));

        public static GerarPropostaRequest CriarRequest() => _requestFaker.Generate();

        public static ClienteCadastradoEvent CriarClienteCadastradoEvent()
        {
            var faker = new Faker("pt_BR");
            return new ClienteCadastradoEvent(
                ClienteId: faker.Random.Guid(),
                Nome: faker.Person.FullName,
                Cpf: faker.Person.Cpf(),
                Email: faker.Internet.Email(),
                RendaMensal: faker.Finance.Amount(1000, 20000),
                DataNascimento: faker.Date.Between(
                    DateTime.Today.AddYears(-80), DateTime.Today.AddYears(-18)),
                OcorridoEm: DateTime.UtcNow);
        }
    }
}
