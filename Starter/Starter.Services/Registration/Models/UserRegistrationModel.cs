using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.Registration.Models
{
    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}