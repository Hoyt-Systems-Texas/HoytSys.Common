namespace Mrh.Concurrent.Agent
{
    public interface IAgentStore<TContext> where TContext: IAgentContext
    {
        AgentMonad<TContext, T> To<T>(long id);

        AgentMonad<TContext, int> To(long id);
    }
}