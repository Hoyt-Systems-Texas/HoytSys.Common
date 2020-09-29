using System.Threading.Tasks;

namespace A19.Concurrent.Agent
{
    public interface IAgentStore<TContext> where TContext: IAgentContext
    {
        Task<IAgentMonad<TContext, T>> To<T>(long id, T value);

        Task<IAgentMonad<TContext, int>> To(long id);
    }
}