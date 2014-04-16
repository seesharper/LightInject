using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightInject.SignalR.Tests
{
    using LightInject.SampleLibrary;

    using Moq;

    [TestClass]
    public class DependencyResolverTests
    {
        [TestMethod]
        public void GetService_ServiceRegisteredThroughAdapter_ReturnsInstance()
        {
            var container = new ServiceContainer();
            var resolver = new LightInjectDependencyResolver(container);
            resolver.Register(typeof(IFoo), () => new Foo());

            var instance = resolver.GetService(typeof(IFoo));

            Assert.IsInstanceOfType(instance, typeof(IFoo));
        }

        [TestMethod]
        public void GetService_ServiceRegisteredWithContainer_ReturnsInstance()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();            
            var resolver = new LightInjectDependencyResolver(container);           
            
            var instance = resolver.GetService(typeof(IFoo));

            Assert.IsInstanceOfType(instance, typeof(IFoo));
        }

        [TestMethod]
        public void Dispose_Adapter_DisposesContainer()
        {
            var containerMock = new Mock<IServiceContainer>();

            using (new LightInjectDependencyResolver(containerMock.Object))
            {                
            }

            containerMock.Verify(c => c.Dispose(), Times.Once);            
        }
    }
}
