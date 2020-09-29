using System.ComponentModel;
using System.Text;
using A19.StateMachine.PSharpBase.Distributed.Messages;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
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
            var header = new MessageHeader();
            header.Wrap(directBuffer, 0, RegisterServerRq.SchemaVersion);
            header.BlockLength = RegisterServerRq.BlockLength;
            header.SchemaId = RegisterServerRq.SchemaId;
            header.TemplateId = RegisterServerRq.TemplateId;
            header.Version = RegisterServerRq.SchemaVersion;

            var currentOffset = MessageHeader.Size;
            var registerServer = new RegisterServerRq();
            registerServer.WrapForEncode(directBuffer, currentOffset);
            var hostName = "Hello";
            registerServer.Server = 2;
            registerServer.SetUri(Encoding.UTF8.GetBytes(hostName), 0, hostName.Length);
            
            header.Wrap(directBuffer, 0, RegisterServerRq.SchemaVersion);
            var decodeServer = new RegisterServerRq();
            currentOffset = MessageHeader.Size;
            decodeServer.WrapForDecode(new DirectBuffer(buffer), currentOffset, header.BlockLength, header.Version);
            Assert.AreEqual(2, decodeServer.Server);
            var hostByte = new byte[10000];
            var length = decodeServer.GetUri(hostByte, 0, hostByte.Length);
            Assert.AreEqual(5, length);
            Assert.AreEqual("Hello", Encoding.UTF8.GetString(hostByte, 0, length));
        }
    }
}