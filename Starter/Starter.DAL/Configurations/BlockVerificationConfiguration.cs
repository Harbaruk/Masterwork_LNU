using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starter.DAL.Entities;

namespace Starter.DAL.Configurations
{
    public class BlockVerificationConfiguration : IEntityTypeConfiguration<BlockVerificationEntity>
    {
        public void Configure(EntityTypeBuilder<BlockVerificationEntity> builder)
        {
            builder.ToTable("BlockVerifications");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserPublicKey)
                .IsRequired();

            builder.HasOne(x => x.Block)
                .WithMany(x => x.Verifications);
        }
    }
}