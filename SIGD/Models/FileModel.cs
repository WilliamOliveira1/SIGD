using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGD.Models
{
    public class FileModel
    {
        [ForeignKey("UserUpload")]
        public Guid UserUploadId { get; set; }
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }        
        public virtual ActivationAccount UserUpload { get; set; }
        public virtual List<PrincipalFileModelView> PrincipalsFiles { get; set; }
    }
}
