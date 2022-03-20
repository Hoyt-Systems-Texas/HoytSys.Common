using System.Threading.Tasks;

namespace A19.Database.Diff
{
    public interface IUpdateRecords<TUserId>
    {
        
        /// <summary>
        ///     The id of the node to perform the update.
        /// </summary>
        int NodeId { get; }
        
        /// <summary>
        ///     Called to run the update.
        /// </summary>
        Task Run(TUserId userId);
    }
}