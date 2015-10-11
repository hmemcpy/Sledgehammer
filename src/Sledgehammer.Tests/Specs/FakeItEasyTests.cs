using FakeItEasy;
using NUnit.Framework;
using Sledgehammer.Tests.TestTypes;

namespace Sledgehammer.Tests.Specs
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
        public void CallTo()
        {
            A.CallTo(() => StaticClass.WithStaticIntMethod());

            Assert.True(SledgehammerInterceptor.IsIntercepted(() => StaticClass.WithStaticIntMethod()));
        }
    }
}