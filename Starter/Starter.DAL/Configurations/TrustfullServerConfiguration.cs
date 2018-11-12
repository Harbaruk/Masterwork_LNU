using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starter.DAL.Entities;

namespace Starter.DAL.Configurations
{
    public class TrustfullServerConfiguration : IEntityTypeConfiguration<TrustfullServerEntity>
    {
        public void Configure(EntityTypeBuilder<TrustfullServerEntity> builder)
        {
            builder.ToTable("TrustfullServers");
            builder.HasKey(x => x.Hash);

            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.PublicKey).IsRequired();
            builder.Property(x => x.Salt).IsRequired();
        }
    }
}