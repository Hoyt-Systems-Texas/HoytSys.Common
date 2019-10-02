using A19.System.Rest;
using A19.User.Common;
using NUnit.Framework;

namespace A19.User.Rest.Test
{
    [TestFixture]
    public class SystemRestClientTest
    {
        
        private readonly SystemClient client = new SystemClient(new UserClientSettingsTest());

        [Test]
        public void UserSystemAuthTest()
        {
            var passCode = "WIYa2NCiC7oqplwEi6/cULCCTa2qwxV5K2t80A+PutFZyCf/XhhDJSvYApWU0/v+miW8V/S+2na16OxgOcrSDA==";
            var systemId = 3;

            var systemT = client.Login(new SystemLoginRq
            {
                PassCode = passCode,
                SystemId = systemId,
                AccessingSystemId = 1
            });
            systemT.Wait();
            var system = systemT.Result;
        }
    }
}