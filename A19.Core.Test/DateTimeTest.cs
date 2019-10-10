using System;
using NUnit.Framework;

namespace A19.Core.Test
{
    
    [TestFixture]
    public class DateTimeTest
    {
        [Test]
        public void CreateTest()
        {
            var date = new DateTime(2019, 1, 2, 12, 0, 0);
            var mils = date.ToUnix();
            var from = DateTimeExt.FromUnix(mils);
            Assert.AreEqual(from, date.ToUniversalTime());
        }
    }
}