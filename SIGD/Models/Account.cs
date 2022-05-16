using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace SIGD.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string Email { get; set; }
        public SecureString Password { get; set; }
    }
}
