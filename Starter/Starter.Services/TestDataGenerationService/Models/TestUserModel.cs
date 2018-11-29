using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.TestDataGenerationService.Models
{
    public class TestUserModel
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}