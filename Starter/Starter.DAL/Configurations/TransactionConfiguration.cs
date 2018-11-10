using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starter.DAL.Entities;

namespace Starter.DAL.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<TransactionEntity>
    {
        public void Configure(EntityTypeBuilder<TransactionEntity> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Date);
            builder.Property(x => x.Description);
            builder.Property(x => x.State).IsRequired();

            builder.HasOne(x => x.Initiator)
                .WithMany(x => x.Transactions);

            builder.HasOne(x => x.FromAccount).WithMany(x => x.SentTransactions);
            builder.HasOne(x => x.ToAccount).WithMany(x => x.ReceivedTransactions);

            builder.HasOne(x => x.Block)
                .WithMany(x => x.Transactions);
        }
    }
}