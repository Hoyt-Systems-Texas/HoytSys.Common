using System;
using NUnit.Framework;

namespace Mrh.Messaging.Test
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
            var conn2 = Guid.NewGuid();
            connectionCollection.AddOrUpdate(
                "test1",
                conn1);
            connectionCollection.AddOrUpdate(
                "test2",
                conn2);

            ConnectionCollection<Guid>.ConnectionNode node;
            Assert.IsTrue(connectionCollection.GetConnection(
                "test1", out node));
            Assert.AreEqual(conn1, node.ExternalConnection);
        }
    }
}