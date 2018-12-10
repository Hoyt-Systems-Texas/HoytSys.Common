using System.Threading.Tasks;

namespace Mrh.Concurrent.Agent
{
    public interface IAgentContextStore<TContext> where TContext: IAgentContext
    {
        Task<TContext> Lookup(long id);
    }
}