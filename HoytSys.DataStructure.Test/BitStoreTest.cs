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
            bitStore.WriteUint(0, 93);
            bitStore.WriteUint(1, maxValue);
            bitStore.WriteUint(2, 13424);
            
            Assert.AreEqual((uint) 93, bitStore.ReadUint(0));
            Assert.AreEqual(maxValue, bitStore.ReadUint(1));
            Assert.AreEqual(13424, bitStore.ReadUint(2));
            
            // Wipe out all of the 1s with 0s to make sure it workes.
            bitStore.WriteUint(1, 0);
            Assert.AreEqual(0, bitStore.ReadUint(1));
        }
        
        /// <summary>
        ///     A simple test that doesn't test the boundaries.
        /// </summary>
        [Test]
        public void AcrossBoundary()
        {
            var bitStore = new BitStore(7, 20);
            var maxValue = (uint) (1 << 20) - 1;
            bitStore.WriteUint(0, 93);
            bitStore.WriteUint(1, maxValue);
            bitStore.WriteUint(2, 13424);
            bitStore.WriteUint(3, maxValue);
            bitStore.WriteUint(4, 13424);
            bitStore.WriteUint(5, 13424);
            bitStore.WriteUint(6, 13424);
            bitStore.WriteUint(7, 13424);
            
            Assert.AreEqual((uint) 93, bitStore.ReadUint(0));
            Assert.AreEqual(maxValue, bitStore.ReadUint(1));
            Assert.AreEqual(13424, bitStore.ReadUint(2));
            Assert.AreEqual(maxValue, bitStore.ReadUint(3));
            Assert.AreEqual(13424, bitStore.ReadUint(4));
            Assert.AreEqual(13424, bitStore.ReadUint(5));
            Assert.AreEqual(13424, bitStore.ReadUint(6));
            Assert.AreEqual(13424, bitStore.ReadUint(7));
            
            // Wipe out all of the 1s with 0s to make sure it workes.
            bitStore.WriteUint(1, 0);
            Assert.AreEqual(0, bitStore.ReadUint(1));
        }
    }
}