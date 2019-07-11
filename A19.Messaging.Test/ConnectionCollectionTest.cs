using System;
using A19.Messaging.Common;
using NUnit.Framework;

namespace A19.Messaging.Test
{
    [TestFixture]
    public class ConnectionCollectionTest
    {
        [Test]
        public void AddOrUpdateTest()
        {
            var connectionCollection = new ConnectionCollection<Guid>(
                new TimeSpan(0, 10, 0));
            var conn1 = Guid.NewGuid();
            var int1 = Guid.NewGuid();
            var conn2 = Guid.NewGuid();
            var int2 = Guid.NewGuid();
            connectionCollection.AddOrUpdate(
                int1,
                conn1);
            connectionCollection.AddOrUpdate(
                int2,
                conn2);

            ConnectionCollection<Guid>.ConnectionNode node;
            Assert.IsTrue(connectionCollection.GetConnection(
                int1, out node));
            Assert.AreEqual(conn1, node.ExternalConnection);
        }
    }
}