using System;
using A19.User.Common;
using NUnit.Framework;

namespace A19.User.Rest.Test
{
    [TestFixture]
    public class UserClientTest
    {
        private IUserClient _userClient = new UserClient(new UserClientSettingsTest());

        [Test]
        public void GetTest()
        {
            var uT = _userClient.Get(Guid.Parse("ECD85692-1BE6-4938-BCEC-252D6D5A1770"));
            uT.Wait();
            var u = uT.Result;
        }
    }
}