using System.Threading.Tasks;

namespace A19.Concurrent.Agent
{
    public interface IAgentContextStore<TContext> where TContext: IAgentContext
    {
        Task<TContext> Lookup(long id);
    }
}