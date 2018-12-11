using System;

namespace Mrh.Concurrent.Agent
{
    public class AgentRemovedAfterTimeoutException : Exception
    {
        public AgentRemovedAfterTimeoutException(): base("The agent has timed out and has been unloaded from the system.")
        {
            
        }
        
    }
}