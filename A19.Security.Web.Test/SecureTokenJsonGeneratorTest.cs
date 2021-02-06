using System;
using HoytSys.Core;
using Mrh.Monad;
using NUnit.Framework;

namespace A19.Security.Web.Test
{
    public class SecureTokenJsonGeneratorTest
    {
        private readonly SecureKeyGenerator _secureKeyGenerator = new SecureKeyGenerator();
        private SecureTokenJsonGenerator _secureTokenJsonGenerator;

        [SetUp]
        public void Setup()
        {
            _secureTokenJsonGenerator = new SecureTokenJsonGenerator(
                _secureKeyGenerator,
                _secureKeyGenerator.Generate(64));
        }

        [Test]
        public void EncodeDecodeTest()
        {
            var encode = new TestObj
            {
                Field1 = 1
            };
            var token = _secureTokenJsonGenerator.CreateToken(
                encode,
                DateTime.Now.AddMinutes(1));

            var result = _secureTokenJsonGenerator.DecodeToken<TestObj>(
                token.ToArray());
            result
                .Success(r =>
                {
                    Assert.AreEqual(1, encode.Field1);
                })
                .Error(() => { Assert.Fail("Failed to validate token."); });
        }

        [Test]
        public void ExpireTest()
        {
            var encode = new TestObj
            {
                Field1 = 1
            };
            var token = _secureTokenJsonGenerator.CreateToken(
                encode,
                DateTime.Now.AddMinutes(-1));

            var result = _secureTokenJsonGenerator.DecodeToken<TestObj>(
                token.ToArray());
            result
                .Success(r =>
                {
                    Assert.Fail("Failed to validate token."); 
                })
                .Error(() => { });
        }

        [Test]
        public void InvalidTokenTest()
        {
            var encode = new TestObj
            {
                Field1 = 1
            };
            var token = _secureTokenJsonGenerator.CreateToken(
                encode,
                DateTime.Now.AddMinutes(-1));

            var overrideV = token.Slice(0, 16);
            _secureKeyGenerator.Generate(overrideV);
            var result = _secureTokenJsonGenerator.DecodeToken<TestObj>(
                token.ToArray());
            result
                .Success(r =>
                {
                    Assert.Fail("Failed to validate token."); 
                })
                .Error(() =>
                {
                });
        }

        public class TestObj
        {
            public int Field1 { get; set; }
        }
    }
}