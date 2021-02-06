using System;

namespace HoytSys.Core
{
    public interface IUpdatable
    {
        /// <summary>
        ///     The last time the entity was updated.
        /// </summary>
        DateTime UpdateOn { get; set; }
        
        /// <summary>
        ///     The last user who did the edit.
        /// </summary>
        Guid UpdatedBy { get; set; }
    }
}