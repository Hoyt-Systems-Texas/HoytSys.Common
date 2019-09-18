using System;

namespace A19.Core
{
    public interface ICreated
    {
        /// <summary>
        ///     The user who created the value.
        /// </summary>
        Guid CreatedBy { get; set; }
        
        /// <summary>
        ///     When the entity was created.
        /// </summary>
        DateTime CreatedOn { get; set; }
    }
}