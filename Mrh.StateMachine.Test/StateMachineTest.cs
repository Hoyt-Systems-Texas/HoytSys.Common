using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;

namespace Mrh.StateMachine.Test
{
    [TestFixture]
    public class StateMachineTest
    {
        private InMemoryTransitionStore<FakeStateType, FakeEventType, FakeContext, string> transitionStore;
        private string name = "State Machine Test";

            [SetUp]
        public void SetUp()
        {
            this.transitionStore = new InMemoryTransitionStore<FakeStateType, FakeEventType, FakeContext, string>();
        }

        [Test]
        public void BasicTest()
        {
            var retryHandler = A.Fake<IRetryHandle<FakeStateType, FakeEventType, FakeContext, string>>();
            var backgroundHandler = A.Fake<IBackgroundTransition<FakeStateType, FakeEventType, FakeContext, string>>();
            var stateMachine = new PersistedStateMachine<FakeStateType, FakeEventType,FakeContext, string>(
                this.name,
                retryHandler,
                this.transitionStore,
                backgroundHandler,
                ((exception, type, arg3, arg4) =>
                {
                }));
            stateMachine
                .Add(new NewState())
                .Add(new WaitingState());
            
            var context = new FakeContext();
            context.CurrentState = FakeStateType.New;

            stateMachine.Transition(
                FakeEventType.Persist,
                context,
                "Hi").Wait();
            
            Assert.IsNotEmpty(this.transitionStore.Transitions);
        }

        public enum FakeStateType
        {
            New = 0,
            Waiting = 1
        }

        public enum FakeEventType
        {
            Persist = 1
        }

        public class FakeContext : IEventContext<FakeStateType>
        {
            public FakeStateType CurrentState { get; set; }
        }

        public class NewState : IState<FakeStateType, FakeEventType, FakeContext, string>
        {
            public FakeStateType State => FakeStateType.New;
            
            public IEnumerable<ValidTransition<FakeStateType, FakeEventType>> SupportedTransitions => new[]
            {
                ValidTransition<FakeStateType, FakeEventType>.To(FakeEventType.Persist, FakeStateType.Waiting),
            };
            
            public Task<TransitionResult<FakeEventType>> Entry(FakeEventType changeEvent, FakeContext eventContext, string message)
            {
                throw new System.NotImplementedException();
            }

            public Task Exit(FakeEventType changeEvent, FakeContext eventContext, string message)
            {
                return Task.FromResult(0);
            }
        }

        public class WaitingState : IState<FakeStateType, FakeEventType, FakeContext, string>
        {
            public FakeStateType State => FakeStateType.Waiting;

            public IEnumerable<ValidTransition<FakeStateType, FakeEventType>> SupportedTransitions => new List<ValidTransition<FakeStateType, FakeEventType>>(0);
            
            public Task<TransitionResult<FakeEventType>> Entry(FakeEventType changeEvent, FakeContext eventContext, string message)
            {
                return Task.FromResult<TransitionResult<FakeEventType>>(TransitionResult<FakeEventType>.Stop());
            }

            public Task Exit(FakeEventType changeEvent, FakeContext eventContext, string message)
            {
                return Task.FromResult(0);
            }
        }
    }
}