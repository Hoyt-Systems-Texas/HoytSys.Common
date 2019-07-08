
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mrh.Concurrent.StateMachine;
using NUnit.Framework;


namespace Mrh.Concurrent.Test.StateMachine
{
    using TS = EventActionTypeExt<
        TestState,
        TestEvent,
        TestStateCtx,
        string>;
    
    public enum TestState
    {
        A,
        B,
        C
    }

    public enum TestEvent
    {
        E,
        F,
        G,
        H,
        I
        
    }

    public class TestStateCtx : AbstractStateCtx<
        TestState, TestEvent, string>
    {
        public bool Ran;
    }

    [TestFixture]
    public class StateMachineTest
    {

        [Test]
        public void GoToStateTest()
        {
            var ctx = new TestStateCtx();
            var stateMachine = new StateMachine<TestState, TestEvent, TestStateCtx, string>();

            stateMachine.Add(new StateA());
            stateMachine.RegisterCtx(ctx);
            ctx.Add(TestEvent.E, "hi");
            
            Thread.Sleep(5);
            Assert.AreEqual(TestState.B, ctx.CurrentState);
        }

        [Test]
        public void ActionTest()
        {
            var ctx = new TestStateCtx();
            var stateMachine = new StateMachine<TestState,TestEvent,TestStateCtx,string>();
            stateMachine.Add(new StateA());
            stateMachine.RegisterCtx(ctx);
            ctx.Add(TestEvent.F, "hi");
            ctx.Add(TestEvent.E, "hello");
            Thread.Sleep(5);
            Assert.IsTrue(ctx.Ran);
            Assert.AreEqual(TestState.B, ctx.CurrentState);
            
        }
        
        private class StateA : BaseState<TestState, TestEvent, TestStateCtx, string>
        {
            public override TestState State => TestState.A;

            public override IEnumerable<EventNode<TestState, TestEvent, TestStateCtx, string>> Events => new[]
            {
                TS.GoTo(TestEvent.E, TestState.B),
                TS.Do(TestEvent.F, new ActionF())
            };
        }

        private class ActionF : IAction<TestState, TestEvent, TestStateCtx, string>
        {

            public bool Run = false;
            
            public Task Execute(TestEvent @event, TestStateCtx ctx, string param)
            {
                ctx.Ran = true;
                return Task.FromResult(0);
            }
        }
    }
}
