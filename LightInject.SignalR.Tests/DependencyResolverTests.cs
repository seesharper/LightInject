using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightInject.SignalR.Tests
{
    using LightInject.SampleLibrary;

    [TestClass]
    public class DependencyResolverTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var container = new ServiceContainer();
            var resolver = new LightInjectDependencyResolver(container);

            resolver.Register(typeof(IFoo), () => new Foo());
        }
    }
}
