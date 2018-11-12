using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.DAL.Entities
{
    public class TrustfullServerEntity
    {
        public string Hash { get; set; }
        public string PublicKey { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
    }
}