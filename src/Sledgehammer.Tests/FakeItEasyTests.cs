using System.IO;
using FakeItEasy;
using NUnit.Framework;

namespace Sledgehammer.Tests
{
    [TestFixture]
    public class FakeItEasyTests
    {
        [SetUp]
        public void Setup()
        {
            Sledgehammer.Use<FakeItEasy>();
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

        [Test]
        public void Throws()
        {
            A.CallTo(() => StaticClass.WithVoidMethod()).Throws(new InvalidDataException("Bad data!"));

            var ex = Assert.Throws<InvalidDataException>(() => StaticClass.WithVoidMethod());

            Assert.AreEqual("Bad data!", ex.Message);
        }

        [Test]
        public void Invokes()
        {
            bool flag = false;
            A.CallTo(() => StaticClass.WithVoidMethod()).Invokes(() => flag = true);

            StaticClass.WithVoidMethod();

            Assert.IsTrue(flag);
        }

        [Test]
        public void Invokes2()
        {
            bool flag = false;
            A.CallTo(() => StaticClass.WithVoidMethod()).Invokes(i => flag = true);

            StaticClass.WithVoidMethod();

            Assert.IsTrue(flag);
        }

    }

    public static class StaticClass
    {
        public static void WithVoidMethod()
        {
        }

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