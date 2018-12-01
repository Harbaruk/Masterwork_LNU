using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starter.DAL.Entities;

namespace Starter.DAL.Configurations
{
    public class BlockConfiguration : IEntityTypeConfiguration<BlockEntity>
    {
        public void Configure(EntityTypeBuilder<BlockEntity> builder)
        {
            builder.ToTable("Blocks");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Hash).IsRequired();
            builder.Property(x => x.BlockState).IsRequired();
            builder.Property(x => x.Date);
            builder.Property(x => x.Nonce);
            builder.Property(x => x.PreviousBlockHash).IsRequired();

            builder.HasOne(x => x.Miner);
            builder.HasMany(x => x.Transactions).WithOne(x => x.Block);
            builder.HasMany(x => x.Verifications).WithOne(x => x.Block);
        }
    }
}