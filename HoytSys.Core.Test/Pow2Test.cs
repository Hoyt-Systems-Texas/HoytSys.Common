using NUnit.Framework;

namespace HoytSys.Core.Test
{
    [TestFixture]
    public class Pow2Test
    {
        [Test]
        public void MinBits7()
        {
            var upTest = Pow2.MinimumBits(7);
            Assert.AreEqual(3, upTest);
        }
        
        [Test]
        public void MinBits16()
        {
            var upTest = Pow2.MinimumBits(16);
            Assert.AreEqual(4, upTest);
        }
    }
}