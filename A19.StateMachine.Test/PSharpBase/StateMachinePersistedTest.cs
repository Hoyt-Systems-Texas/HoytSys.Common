using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using A19.Concurrent;
using A19.Concurrent.StateMachine;
using A19.StateMachine.PSharpBase;
using FakeItEasy;
using NUnit.Framework;

namespace Mrh.StateMachine.Test.PSharpBase
{
    [TestFixture]
    public class StateMachinePersistedTest
    {
        public enum TestState
        {
            A,
            B,
            C,
            D
        }

        public enum TestEvent
        {
            E,
            F,
            G,
            H,
            I
        }

        [Test]
        public void AToB()
        {
            var stateMachineD = CreateStateMachine();
            var stateMachine = stateMachineD.Item1;
            var ctx = stateMachineD.Item2;

            ctx.Add(new EventActionNodePersist<long, TestState, TestEvent, string, Context, Guid>
            {
                Event = TestEvent.E,
                Id = 1,
            }).Wait();
            ctx.Add(new EventActionNodePersist<long, TestState, TestEvent, string, Context, Guid>
            {
                Event = TestEvent.F,
                Id = 1
            }).Wait();
            ctx.Add(new EventActionNodePersist<long, TestState, TestEvent, string, Context, Guid>
            {
                Event = TestEvent.G,
            }).Wait();
            var ran = false;
            Assert.AreEqual(TestState.B, ctx.CurrentState);
            Assert.IsTrue(ctx.F);
        }

        public (StateMachinePersisted<long, TestState, TestEvent, Context, string, Guid>, Context) CreateStateMachine()
        {
            var store = A.Fake<IEventPersistedStore<long, TestState, TestEvent, string, Context, Guid>>();
            var retry = A.Fake<IRetryService>();
            var stateMachine = new StateMachinePersisted<long, TestState, TestEvent, Context, string, Guid>(
                A.Fake<IEventPersistedStore<long, TestState, TestEvent, string, Context, Guid>>(),
                A.Fake<IRetryService>());
            stateMachine
                .Add(new StateA())
                .Add(new StateB())
                ;
            var newEventReceived = new NewEventReceived<long, TestState, TestEvent, Context, string, Guid>(
                stateMachine);
            var ctx = new Context(
                1,
                TestState.A,
                32,
                store,
                retry,
                newEventReceived);


            return (stateMachine, ctx);
        }

        public class Context : AbstractStateMachinePersistCtx<long, TestState, TestEvent, string, Context, Guid>
        {
            public bool F;

            public Context(
                long stateMachineKey,
                TestState currentState,
                uint size,
                IEventPersistedStore<long, TestState, TestEvent, string, Context, Guid> eventPersistedStore,
                IRetryService retryService,
                NewEventReceived<long, TestState, TestEvent, Context, string, Guid> newEventReceived,
                int maxDelay = 600) : base(
                stateMachineKey,
                currentState,
                size,
                eventPersistedStore,
                retryService,
                newEventReceived,
                maxDelay)
            {
            }
        }

        public class StateA : IStatePersisted<long, TestState, TestEvent, Context, string, Guid>
        {
            public TestState State => TestState.A;

            public IEnumerable<EventNodePersisted<long, TestState, TestEvent, Context, string, Guid>> Events =>
                new EventNotePersistedBuilder<long, TestState, TestEvent, Context, string, Guid>()
                    .GoTo(TestEvent.E, TestState.B)
                    .Build();

            public Task Entry(TestEvent @event, Guid userId, Context ctx, string param)
            {
                return Task.FromResult(0);
            }

            public Task Exit(TestEvent @event, Guid userId, Context ctx, string param)
            {
                return Task.FromResult(0);
            }
        }

        public class StateB : IStatePersisted<long, TestState, TestEvent, Context, string, Guid>
        {
            public TestState State => TestState.B;

            public IEnumerable<EventNodePersisted<long, TestState, TestEvent, Context, string, Guid>> Events =>
                new EventNotePersistedBuilder<long, TestState, TestEvent, Context, string, Guid>()
                    .Do(TestEvent.F, new ActionF())
                    .Ignore(TestEvent.H)
                    .Build();

            public Task Entry(TestEvent @event, Guid userId, Context ctx, string param)
            {
                return Task.FromResult(0);
            }

            public Task Exit(TestEvent @event, Guid userId, Context ctx, string param)
            {
                return Task.FromResult(0);
            }
        }

        public class ActionF : IAction<long, TestState, TestEvent, Context, string, Guid>
        {
            public Task Run(TestEvent @event, Guid userId, Context ctx, string param)
            {
                ctx.F = true;
                return Task.FromResult(0);
            }
        }
    }
}