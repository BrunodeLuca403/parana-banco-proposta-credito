using Microsoft.EntityFrameworkCore;
using PropostaCredito.Core.Entitys;
using PropostaCredito.Core.Repository;
using PropostaCredito.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropostaCredito.Infrastructure.Repository
{
    public class PropostaRepository : IPropostaRepository
    {
        private readonly PropostaDbContext _db;
        public async Task<Proposta?> BuscarProposta(Guid ClientId)
        {
            return await _db.Propostas.SingleOrDefaultAsync(p => p.IdCliente == ClientId);
        }

        public async Task GerarProposta(Proposta proposta)
        {
            await _db.Propostas.AddAsync(proposta);
            await _db.SaveChangesAsync();
        }
    }
}
