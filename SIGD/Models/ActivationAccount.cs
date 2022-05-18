using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Models
{
    /// <summary>
    /// Activation Account model
    /// </summary>
    public class ActivationAccount
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }

        public string ActivationToken { get; set; }
        public DateTime TokenCreation { get; set; }
        public bool IsDueTime { get; set; }
        public bool IsActivated { get; set; }
    }
}
