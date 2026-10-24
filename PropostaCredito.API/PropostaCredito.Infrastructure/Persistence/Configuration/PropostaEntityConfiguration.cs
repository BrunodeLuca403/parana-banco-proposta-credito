using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropostaCredito.Core.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PropostaCredito.Infrastructure.Persistence.Configuration
{
    public class PropostaEntityConfiguration : IEntityTypeConfiguration<Proposta>
    {
        public void Configure(EntityTypeBuilder<Proposta> builder)
        {
            builder.ToTable("Propostas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IdCliente)
            .IsRequired();

            builder.Property(x => x.Score)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.LimitePorCartao)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.QuantidadeCartoes)
                .IsRequired();

            builder.Property(x => x.CreatAt)
                .HasColumnType("datetime")
                .IsRequired();

            builder.HasIndex(x => x.IdCliente)
                .IsUnique();

        }
    }
}
