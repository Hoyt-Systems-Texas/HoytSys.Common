using NUnit.Framework;

namespace A19.Security.User.Test
{
    [TestFixture]
    public class BCryptPasswordHashingTest
    {
        
        private BCryptPasswordHashing bCryptPasswordHashing = new BCryptPasswordHashing();

        [Test]
        public void HashTest()
        {
            var password = "Password1";
            var hashed = bCryptPasswordHashing.Hash(password);
            Assert.IsTrue(bCryptPasswordHashing.Verify(password, hashed));
        }
    }
}