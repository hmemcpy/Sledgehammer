using System.IO;
using FakeItEasy;
using NUnit.Framework;
using Sledgehammer.Tests.TestTypes;

namespace Sledgehammer.Tests.FakeItEasy
{
    [TestFixture]
    public class FakeStaticMethodsTests
    {
        [SetUp]
        public void Setup()
        {
            Sledgehammer.Use<global::Sledgehammer.FakeItEasy>();
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

        [Test]
        public void MustHaveHappened()
        {
            A.CallTo(() => StaticClass.WithStaticStringMethod()).Returns("Hello");

            StaticClass.WithStaticStringMethod();

            A.CallTo(() => StaticClass.WithStaticStringMethod()).MustHaveHappened();
        }

        [Test]
        public void MustHaveHappenedTwice()
        {
            A.CallTo(() => StaticClass.WithStaticStringMethod()).Returns("Hello");

            StaticClass.WithStaticStringMethod();
            StaticClass.WithStaticStringMethod();

            A.CallTo(() => StaticClass.WithStaticStringMethod()).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Test]
        public void MustNotHaveHappened()
        {
            A.CallTo(() => StaticClass.WithStaticStringMethod()).Returns("Hello");

            StaticClass.WithStaticStringMethod();

            A.CallTo(() => StaticClass.WithVoidMethod()).MustNotHaveHappened();
        }

        [Test]
        public void Invokes_with_arguments()
        {
            bool flag = false;
            A.CallTo(() => StaticClass.WithVoidMethodArgs(A<int>.Ignored)).Invokes(i => flag = true);

            StaticClass.WithVoidMethodArgs(0);

            Assert.IsTrue(flag);
        }

        [Test]
        public void CleanStart()
        {
            var method = Info.OfMethod("Sledgehammer.Tests", "Sledgehammer.Tests.TestTypes.StaticClass", "WithStaticIntMethod");
            A.CallTo(() => StaticClass.WithStaticIntMethod()).Returns(2);

            Assert.AreEqual(0, MockManager.GetManager(method).Invocations);
        }

        [Test]
        public void Falls_back_to_FIE_functionality_for_interfaces()
        {
            var fake = A.Fake<IFoo>();
            A.CallTo(() => fake.GetValue()).Returns(3);

            Assert.AreEqual(3, fake.GetValue());
        }
    }
}