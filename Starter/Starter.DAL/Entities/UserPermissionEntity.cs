using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.DAL.Entities
{
    public class UserPermissionEntity
    {
        public int Id { get; set; }
        public UserEntity User { get; set; }
        public PermissionEntity Permission { get; set; }
    }
}