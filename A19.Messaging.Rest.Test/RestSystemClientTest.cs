using A19.User.Common;
using Mrh.Monad;
using NUnit.Framework;

namespace A19.Messaging.Rest.Test
{
    [TestFixture]
    public class RestSystemClientTest
    {
        private RestSystemClient _restSystemClient = new RestSystemClient(
            "https://localhost:5002/api");

        [Test]
        public void LoginTest()
        {
            var resultT = _restSystemClient.PostAsync<UserLoginRq, UserLoginRs>(
                "Login",
                "Login",
                new UserLoginRq
                {
                    Password = "Test",
                    Username = "mrh0057",
                    SystemId = 1,
                    IpAddress = "127.0.0.1"
                });
            resultT.Wait();
            var result = resultT.Result;
            var success = false;
            result.Bind(a =>
            {
                success = a.Success;
                return a.ToResultMonad();
            });
            Assert.IsTrue(success);
        }
    }
}