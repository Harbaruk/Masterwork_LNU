using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.DAL.Entities
{
    public class RefreshTokenEntity
    {
        public int Id { get; set; }
        public UserEntity User { get; set; }
        public string Token { get; set; }
    }
}