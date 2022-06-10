using System;
using System.Collections.Generic;

namespace SIGD.Models
{
    public class FileModel
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public virtual ActivationAccount UserUpload { get; set; }
        public virtual List<ActivationAccount> UsersToRead { get; set; }
    }
}
