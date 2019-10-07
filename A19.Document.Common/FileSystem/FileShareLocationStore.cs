using System;
using System.Collections.Generic;
using A19.Core;

namespace A19.Document.Common.FileSystem
{
    public class FileShareLocationStore : IValidatable
    {
        public int FileShareLocationStoreId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string RootPath { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public List<ValidationError> Validate(List<ValidationError> errors)
        {
            errors
                .MaxLength(
                    100,
                    nameof(Name),
                    Name)
                .Required(
                    nameof(Name),
                    Name)
                .MaxLength(
                    200,
                    nameof(Description),
                    Description)
                .Required(
                    nameof(Description),
                    Description)
                .MaxLength(
                    1000,
                    nameof(RootPath),
                    RootPath)
                .Required(
                    nameof(RootPath),
                    RootPath);
            return errors;
        }
    }
}