using System.Collections.Generic;

namespace A19.Document.Common
{
    public class MimeTypeModel
    {

        public MimeTypeModel()
        {
            Ext = new List<string>(1);
        }
        
        public MimeType MimeType { get; set; }
        
        public string Name { get; set; }
        
        public string ContentType { get; set; }
        
        public List<string> Ext { get; set; }
    }
}