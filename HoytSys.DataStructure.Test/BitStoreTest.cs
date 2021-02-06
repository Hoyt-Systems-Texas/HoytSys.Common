using HoytSys.DataStructures;
using NUnit.Framework;

namespace HoytSys.DataStructure.Test
{
    [TestFixture]
    public class BitStoreTest
    {

        /// <summary>
        ///     A simple test that doesn't test the boundaries.
        /// </summary>
        [Test]
        public void SimpleTest()
        {
            var bitStore = new BitStore(6, 20);
            var maxValue = (uint) (1 << 20) - 1;
            bitStore.Write(0, 93);
            bitStore.Write(1, maxValue);
            bitStore.Write(2, 13424);
            
            Assert.AreEqual((uint) 93, bitStore.Read(0));
            Assert.AreEqual(maxValue, bitStore.Read(1));
            Assert.AreEqual(13424, bitStore.Read(2));
            
            // Wipe out all of the 1s with 0s to make sure it workes.
            bitStore.Write(1, 0);
            Assert.AreEqual(0, bitStore.Read(1));
        }
        
        /// <summary>
        ///     A simple test that doesn't test the boundaries.
        /// </summary>
        [Test]
        public void AcrossBoundary()
        {
            var bitStore = new BitStore(7, 20);
            var maxValue = (uint) (1 << 20) - 1;
            // Random writes.
            bitStore.Write(0, 93);
            bitStore.Write(5, 13424);
            bitStore.Write(2, 13424);
            bitStore.Write(1, maxValue);
            bitStore.Write(3, maxValue);
            bitStore.Write(7, 13424);
            bitStore.Write(6, 13424);
            bitStore.Write(4, 13424);
            
            Assert.AreEqual((uint) 93, bitStore.Read(0));
            Assert.AreEqual(maxValue, bitStore.Read(1));
            Assert.AreEqual(maxValue, bitStore.Read(3));
            Assert.AreEqual(13424, bitStore.Read(4));
            Assert.AreEqual(13424, bitStore.Read(7));
            Assert.AreEqual(13424, bitStore.Read(2));
            Assert.AreEqual(13424, bitStore.Read(5));
            Assert.AreEqual(13424, bitStore.Read(6));
            Assert.AreEqual(13424, bitStore.Read(7));
            Assert.AreEqual(13424, bitStore.Read(5));
            Assert.AreEqual(13424, bitStore.Read(6));
            
            // Wipe out all of the 1s with 0s to make sure it workes.
            bitStore.Write(1, 0);
            Assert.AreEqual(0, bitStore.Read(1));
        }
    }
}