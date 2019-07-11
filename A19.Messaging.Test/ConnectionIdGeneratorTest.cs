using System;
using NUnit.Framework;

namespace A19.Messaging.Test
{
    [TestFixture]
    public class ConnectionIdGeneratorTest
    {

        private readonly ConnectionIdGenerator connectionIdGenerator = new ConnectionIdGenerator();

        [Test]
        public void GenerateTest()
        {
            var guid = this.connectionIdGenerator.Generate();
            Assert.AreNotEqual(Guid.Empty, guid);
            
        }
    }
}