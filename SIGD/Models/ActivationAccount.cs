using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Models
{
    public enum Role
    {
        administrator,
        diretor
    }

    /// <summary>
    /// Activation Account model
    /// </summary>
    public class ActivationAccount
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string password { get; set; }
        public DateTime passwordExpiration { get; set; }
        public bool IsActivated { get; set; }
        public Role role { get; set; }
    }
}
