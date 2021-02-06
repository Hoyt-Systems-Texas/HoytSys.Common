using System.Collections.Generic;
using HoytSys.Core;

namespace A19.Document.Common
{
    public class DocumentType : IValidatable
    {
        public int DocumentTypeId { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public List<ValidationError> Validate(List<ValidationError> errors)
        {
            errors
                .Required(nameof(Name), Name)
                .MaxLength(100, nameof(Name), Name)
                .Required(nameof(Description), Description)
                .MaxLength(200, nameof(Description), Description)
                ;
            return errors;
        }
    }
}