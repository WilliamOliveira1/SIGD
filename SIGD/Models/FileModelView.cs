using System.Collections.Generic;


namespace SIGD.Models
{
    public class FileModelView
    {
        public string FileName { get; set; }
        public List<string> ListOfReaders { get; set; }
        public string UserUpload { get; set; }
    }
}
