using System;
using System.Collections.Generic;

namespace A19.Document.Common
{
    public class Document 
    {
        /// <summary>
        ///     The id of the document.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        ///     The current version of the document.
        /// </summary>
        public Guid DocumentVersionId { get; set; }

        /// <summary>
        ///     The owner of the document.
        /// </summary>
        public Owner Owner { get; set; }

        /// <summary>
        ///     The name of the document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The tags for the document.
        /// </summary>
        public List<Tag> Tags { get; set; }

        /// <summary>
        ///     The mime type of the document.
        /// </summary>
        public MimeType MimeType { get; set; }

        /// <summary>
        ///     The system who submitted the document.
        /// </summary>
        public int SystemId { get; set; }

        /// <summary>
        ///     The date the document is to be delete.
        /// </summary>
        public DateTime? DeleteOn { get; set; }

        /// <summary>
        ///     When the document is suppose to be removed and archived.
        /// </summary>
        public DateTime? ArchiveOn { get; set; }

        /// <summary>
        ///     true if the document is immutable.
        /// </summary>
        public bool Immutable { get; set; }
        
        public DocumentStatus Status { get; set; }
        
        public string Comment { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdateOn { get; set; }

        public Guid UpdatedBy { get; set; }
    }
}