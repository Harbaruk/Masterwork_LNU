using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Starter.DAL.Entities;

namespace Starter.DAL.Configurations
{
    internal class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermissionEntity>
    {
        public void Configure(EntityTypeBuilder<UserPermissionEntity> builder)
        {
            builder.ToTable("UserPermissions");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Permission);
            builder.HasOne(x => x.User);
        }
    }
}