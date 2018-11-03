using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starter.DAL.Entities;

namespace Starter.DAL.Configurations
{
    public class TwoFactorAuthConfiguration : IEntityTypeConfiguration<TwoFactorAuthEntity>
    {
        public void Configure(EntityTypeBuilder<TwoFactorAuthEntity> builder)
        {
            builder.ToTable("TwoFactorAuth");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Secret).IsRequired();
            builder.Property(x => x.CreatedAt);

            builder.HasOne(x => x.User).WithOne(x => x.TwoFactorAuth);
        }
    }
}