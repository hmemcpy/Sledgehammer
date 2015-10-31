using FakeItEasy;
using NUnit.Framework;
using Sledgehammer.FakeItEasy;

namespace Sledgehammer.Tests
{
    [TestFixture]
    public class FakeItEasyTests
    {
        [SetUp]
        public void Setup()
        {
            Sledgehammer.Use<FakeItEasyContext>();
        }

        [Test]
        public void Returns_int()
        {
            A.CallTo(() => StaticClass.WithStaticIntMethod()).Returns(3);

            var result = StaticClass.WithStaticIntMethod();

            Assert.AreEqual(3, result);
        }
    }

    public static class StaticClass
    {
        public static int WithStaticIntMethod()
        {
            throw new System.NotImplementedException();
        }
    }

}