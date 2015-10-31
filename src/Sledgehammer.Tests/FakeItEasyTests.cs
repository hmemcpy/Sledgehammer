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

        [Test]
        public void Returns_string()
        {
            A.CallTo(() => StaticClass.WithStaticStringMethod()).Returns("Hello");

            var result = StaticClass.WithStaticStringMethod();

            Assert.AreEqual("Hello", result);
        }
    }

    public static class StaticClass
    {
        public static int WithStaticIntMethod()
        {
            throw new System.NotImplementedException();
        }

        public static string WithStaticStringMethod()
        {
            throw new System.NotImplementedException();
        }
    }

}