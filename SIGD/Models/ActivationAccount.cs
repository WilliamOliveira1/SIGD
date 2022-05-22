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

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string password { get; set; }
        public DateTime passwordExpiration { get; set; }
        public bool IsActivated { get; set; }
        public Role role { get; set; }
    }
}
