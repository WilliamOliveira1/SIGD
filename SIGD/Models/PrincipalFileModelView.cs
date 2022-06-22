using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGD.Models
{
    public class PrincipalFileModelView
    {
        [ForeignKey("FileModel")]
        public Guid FileModelId { get; set; }
        public Guid Id { get; set; }
        public string SupervisorName { get; set; }
        public string PrincipalName { get; set; }
        public string PrincipalEmail { get; set; }
        public DateTime LastTimeOpened { get; set; }
        public bool Status { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        [InverseProperty("PrincipalsFiles")]
        public virtual FileModel FileModel { get; set; }
    }
}
