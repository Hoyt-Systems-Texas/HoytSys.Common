using Mrh.Messaging.Json;
using NUnit.Framework;

namespace A19.Messaging.Json.Test
{
    [TestFixture]
    public class JsonBodyReconstructorTest
    {

        [Test]
        public void Test2()
        {
            var bodyReconstructor = new JsonBodyReconstructor(2);
            bodyReconstructor.Append(0, "Hello ");
            bodyReconstructor.Append(1, "World!");
            Assert.IsTrue(bodyReconstructor.Completed());
            Assert.AreEqual("Hello World!", bodyReconstructor.Body);
        }

        [Test]
        public void TestDuplicate()
        {
            var bodyReconstructor = new JsonBodyReconstructor(2);
            bodyReconstructor.Append(0, "Hello ");
            bodyReconstructor.Append(0, "Hello ");
            Assert.IsFalse(bodyReconstructor.Completed());
            bodyReconstructor.Append(1, "World!");
            Assert.IsTrue(bodyReconstructor.Completed());
            Assert.AreEqual("Hello World!", bodyReconstructor.Body);
        }
    }
}