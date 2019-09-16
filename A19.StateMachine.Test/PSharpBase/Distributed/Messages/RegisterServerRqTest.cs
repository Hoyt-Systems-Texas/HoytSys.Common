using System.Text;
using A19.StateMachine.PSharpBase.Distributed.Messages;
using NUnit.Framework;
using Org.SbeTool.Sbe.Dll;

namespace Mrh.StateMachine.Test.PSharpBase.Distributed.Messages
{
    
    [TestFixture]
    public class RegisterServerRqTest
    {

        [Test]
        public void EncodeTest()
        {
            var buffer = new byte[1000];
            var directBuffer = new DirectBuffer(buffer);
            var registerServer = new RegisterServerRq();
            registerServer.WrapForEncode(directBuffer, 0);
            var hostName = "Hello";
            registerServer.Server = 1;
            registerServer.SetUri(Encoding.UTF8.GetBytes(hostName), 0, hostName.Length);
        }
    }
}