using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using A19.Concurrent.StateMachine;
using NUnit.Framework;

namespace A19.Concurrent.Test.StateMachine
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

        public bool RanG;
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

        [Test]
        public void DeferTest()
        {
            var ctx = new TestStateCtx();
            var stateMachine = new StateMachine<TestState,TestEvent,TestStateCtx,string>();
            stateMachine.Add(new StateA());
            stateMachine.Add(new StateB());
            stateMachine.RegisterCtx(ctx);
            ctx.Add(TestEvent.G, "hel");
            ctx.Add(TestEvent.E, "hi");
            Thread.Sleep(15);
            Assert.AreEqual(TestState.B, ctx.CurrentState);
            Assert.IsTrue(ctx.RanG);
        }
        
        private class StateA : BaseState<TestState, TestEvent, TestStateCtx, string>
        {
            public override TestState State => TestState.A;

            public override IEnumerable<EventNode<TestState, TestEvent, TestStateCtx, string>> Events => new[]
            {
                TS.GoTo(TestEvent.E, TestState.B),
                TS.Do(TestEvent.F, new ActionF()),
                TS.Defer(TestEvent.G)
            };
        }

        private class StateB : BaseState<TestState, TestEvent, TestStateCtx, string>
        {
            public override TestState State => TestState.B;

            public override IEnumerable<EventNode<TestState, TestEvent, TestStateCtx, string>> Events => new[]
            {
                TS.Do(TestEvent.G, new ActionG())
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

        private class ActionG : IAction<TestState, TestEvent, TestStateCtx, string>
        {
            public Task Execute(TestEvent @event, TestStateCtx ctx, string param)
            {
                ctx.RanG = true;
                return Task.FromResult(0);
            }
        }
    }
}
