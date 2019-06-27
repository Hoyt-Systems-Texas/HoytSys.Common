using System.Threading.Tasks;

namespace Mrh.Messaging
{
    public interface IRequestIdGenerator
    {
        /// <summary>
        ///     Used to generate the next id.
        /// </summary>
        /// <returns>The id for the message.</returns>
        long Next();

        /// <summary>
        ///     Used to get the last id.
        /// </summary>
        /// <param name="serverId">The id of the server to get the last id for.</param>
        /// <returns>The last id used by the server.</returns>
        Task<long> GetLastId(short serverId);
    }
}