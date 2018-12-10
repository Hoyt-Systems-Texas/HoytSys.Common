using System.Threading.Tasks;

namespace Mrh.Concurrent.Agent
{
    public interface IAgentStore<TContext> where TContext: IAgentContext
    {
        Task<IAgentMonad<TContext, T>> To<T>(long id, T value);

        Task<IAgentMonad<TContext, int>> To(long id);
    }
}