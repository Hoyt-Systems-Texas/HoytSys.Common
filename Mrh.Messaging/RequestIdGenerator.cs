using System.Threading;
using System.Threading.Tasks;

namespace Mrh.Messaging
{
    public class RequestIdGenerator : IRequestIdGenerator
    {

        private readonly int shiftNumber;
        private readonly short serverId;
        private readonly long serverIdShiffted;
        private long currentId;
        

        public RequestIdGenerator(
            IMessageSetting messageSetting)
        {
            this.serverId = messageSetting.ServerId;
            this.shiftNumber = messageSetting.ShiftNumber;
            long server = serverId;
            this.serverIdShiffted = server << this.shiftNumber;
            this.currentId = serverIdShiffted;
        }
        
        public long Next()
        {
            return Interlocked.Increment(ref this.currentId);
        }

        public Task<long> GetLastId(short serverId)
        {
            return Task.FromResult<long>(0);
        }
    }
}