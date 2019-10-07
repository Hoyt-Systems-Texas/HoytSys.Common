using System;
using System.Collections.Generic;
using A19.Core;

namespace A19.Document.Common
{
    public class Tag : IValidatable
    {
        public int TagId { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public DateTime UpdatedOn { get; set; }
        
        public Guid UpdatedBy { get; set; }
        
        public List<ValidationError> Validate(List<ValidationError> errors)
        {
            errors.Required(
                nameof(Name),
                Name)
                .MaxLength(
                    100,
                    nameof(Name),
                    Name,
                    Name)
                .Required(
                    nameof(Description),
                    Description)
                .MaxLength(
                    200,
                    nameof(Description),
                    Description);
            return errors;
        }
    }
}