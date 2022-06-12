using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SIGD.Models
{
    public enum Role
    {
        Administrator,
        Principal
    }

    /// <summary>
    /// Activation Account model
    /// </summary>
    public class ActivationAccount
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public DateTime passwordExpiration { get; set; }
        public bool IsActivated { get; set; }
        public Role role { get; set; }
        public string adminManager { get; set; }
        public virtual List<FileModel> FileModel { get; set; }
    }
}
