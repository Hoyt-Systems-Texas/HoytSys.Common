namespace A19.Document.Common.Messages
{
    public class AddDocumentExtRq
    {
        
        /// <summary>
        ///     The document information.
        /// </summary>
        public Document Document { get; set; }
        
        /// <summary>
        ///     Where the document should be stored.
        /// </summary>
        public FileLocationType Where { get; set; }
        
    }
}