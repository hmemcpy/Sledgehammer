using FakeItEasy;
using NUnit.Framework;
using Sledgehammer.Tests.TestTypes;

namespace Sledgehammer.Tests
{
    [TestFixture]
    public class FakeItEasyTests
    {
        [SetUp]
        public void Setup()
        {
            // todo remove this after fixing appdomain scanning code
            var bootstrapper = A.Fake<IBootstrapper>();

            SledgehammerInterceptor.Use<FakeItEasyContext>();
        }

        [Test]
        public void CallTo_ReturnsLazily()
        {
            A.CallTo(() => StaticClass.WithStaticIntMethod()).ReturnsLazily(() => 3);

            var result = StaticClass.WithStaticIntMethod();

            Assert.AreEqual(3, result);
        }

        [Test]
        public void CallTo_Returns()
        {
            A.CallTo(() => StaticClass.WithStaticIntMethod()).Returns(3);

            var result = StaticClass.WithStaticIntMethod();

            Assert.AreEqual(3, result);
        }
    }
}