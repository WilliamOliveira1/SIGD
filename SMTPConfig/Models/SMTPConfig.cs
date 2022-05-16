using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace SMTPConfig.Models
{
    public class SMTPConfigData
    {
        public int Id { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public string Email { get; set; }
        public string CertName { get; set; }
        public byte[] Password { get; set; }
        public byte[] CertPassword { get; set; }        
    }
}
