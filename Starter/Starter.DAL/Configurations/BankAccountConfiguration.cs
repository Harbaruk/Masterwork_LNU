using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starter.DAL.Entities;

namespace Starter.DAL.Configurations
{
    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccountEntity>
    {
        public void Configure(EntityTypeBuilder<BankAccountEntity> builder)
        {
            builder.ToTable("BankAccounts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Balance);
            builder.Property(x => x.ExpiresAt);
            builder.Property(x => x.OpenedAt);
            builder.Property(x => x.Type);

            builder.HasOne(x => x.Owner)
                .WithMany(x => x.Accounts);
        }
    }
}