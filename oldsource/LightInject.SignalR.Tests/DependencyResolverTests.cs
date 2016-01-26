using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightInject.SignalR.Tests
{
    using System.Linq;

    using LightInject.SampleLibrary;

    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Tracing;

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
        public void GetServices_ServiceRegisteredWithContainer_ReturnsServices()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            var resolver = new LightInjectDependencyResolver(container);

            var instances = resolver.GetServices(typeof(IFoo));

            Assert.AreEqual(1, instances.Count());
        }

        [TestMethod]
        public void GetServices_UnknownService_ReturnsNull()
        {
            var container = new ServiceContainer();            

            var resolver = new LightInjectDependencyResolver(container);

            var instances = resolver.GetServices(typeof(IFoo));

            Assert.IsNull(instances); 
        }

        [TestMethod]
        public void GetServices_ServiceTypeExistsInAdapterAndBase_ReturnsBothInstances()
        {
            var container = new ServiceContainer();                        
            var traceManagerMock = new Mock<ITraceManager>();
            container.RegisterInstance(traceManagerMock.Object);
            var resolver = new LightInjectDependencyResolver(container);            
            
            var instances = resolver.GetServices(typeof(ITraceManager));
            
            Assert.AreEqual(2, instances.Count());
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
