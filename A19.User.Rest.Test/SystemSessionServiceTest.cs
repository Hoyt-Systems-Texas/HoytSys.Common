using System;
using System.Diagnostics;
using System.Threading;
using A19.System.Rest;
using A19.User.Common;
using Mrh.Monad;
using NUnit.Framework;

namespace A19.User.Rest.Test
{
    [TestFixture]
    public class SystemSessionServiceTest
    {
        private SystemSessionService _sessionService = new SystemSessionService(
            new SystemClient(
                new UserClientSettingsTest()),
            new UserClientSettingsTest());

        [Test]
        public void BasicLoadTest()
        {
            string sessionId = null;
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                _sessionService.GetSessionObservable(
                        1,
                        "WIYa2NCiC7oqplwEi6/cULCCTa2qwxV5K2t80A+PutFZyCf/XhhDJSvYApWU0/v+miW8V/S+2na16OxgOcrSDA==")
                    .Subscribe(
                        s => { sessionId = s; });
            });
            var watch = new Stopwatch();
            watch.Start();
            while (sessionId == null && watch.ElapsedMilliseconds < 250)
            {
                Thread.Sleep(2);
            }
            Assert.IsNotNull(sessionId);
            var r = _sessionService.TryGetSession(
                1,
                "",
                out sessionId);
            Assert.IsNotNull(sessionId);
        }
    }
}