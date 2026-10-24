using Microsoft.EntityFrameworkCore;
using PropostaCredito.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PropostaCredito.Infrastructure.Persistence
{
    public class PropostaDbContext : DbContext
    {
        public PropostaDbContext(DbContextOptions<PropostaDbContext> options)
            : base(options)
        {

        }

        public DbSet<Proposta> Propostas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
