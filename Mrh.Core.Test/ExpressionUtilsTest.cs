using NUnit.Framework;

namespace Mrh.Core.Test
{
    [TestFixture]
    public class ExpressionUtilsTest
    {
        public class SampleClass 
        {
            public int IntegerProperty { get; set; }
            public string StringProperty { get; set; }
        }

        [Test]
        public void SetterTest()
        {
            var setterIntegerProperty = ExpressionUtils.CreateSetter<SampleClass, int>(x => x.IntegerProperty);
            var setterStringProperty = ExpressionUtils.CreateSetter<SampleClass, string>(x => x.StringProperty);

            SampleClass sampleClassInstance = new SampleClass();

            setterIntegerProperty(sampleClassInstance, 1);
            setterStringProperty(sampleClassInstance, "2");

            Assert.AreEqual(1, sampleClassInstance.IntegerProperty);
            Assert.AreEqual("2", sampleClassInstance.StringProperty);
        }

        [Test]
        public void GetterTest()
        {
            var getterIntegerProperty = ExpressionUtils.CreateGetter<SampleClass, int>(x => x.IntegerProperty);
            var getterStringProperty = ExpressionUtils.CreateGetter<SampleClass, string>(x => x.StringProperty);

            SampleClass sampleClassInstance = new SampleClass()
            {
                IntegerProperty = 1,
                StringProperty = "2"
            };

            Assert.AreEqual(1, getterIntegerProperty(sampleClassInstance));
            Assert.AreEqual("2", getterStringProperty(sampleClassInstance));
        }

        [Test]
        public void CreateDefaultConstructorTest()
        {
            var defaultConstructor = ExpressionUtils.CreateDefaultConstructor<SampleClass>();
            var setterIntegerProperty = ExpressionUtils.CreateSetter<SampleClass, int>(x => x.IntegerProperty);

            var sampleEntity = defaultConstructor();

            setterIntegerProperty(sampleEntity, 1);

            Assert.AreEqual(1, sampleEntity.IntegerProperty);
        }
    }
}