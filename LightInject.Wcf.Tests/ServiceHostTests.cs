namespace LightInject.Wcf.Tests
{
    using System;
    using LightInject.Interception;
    using LightInject.Tests;
    using LightInject.Wcf.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceHostTests
    {
        [TestMethod]
        public void CreateServiceHost_ServiceTypeIsInterfaceWithServiceContractAttribute_ReturnsServieHostWithProxyAsServiceType()
        {
            var lightInjectServiceHostFactory = new LightInjectServiceHostFactory();
            var serviceType = typeof(IServiceWithServiceContractAttribute);
            var serviceHost = lightInjectServiceHostFactory.CreateServiceHost(serviceType.AssemblyQualifiedName, new Uri[] { });
            var returnedServiceType = serviceHost.Description.ServiceType;
            Assert.IsTrue(typeof(IProxy).IsAssignableFrom(returnedServiceType));
        }

        [TestMethod]
        public void CreateServiceHost_ServiceTypeIsConcrete_ThrowsNotSupportedException()
        {
            var lightInjectServiceHostFactory = new LightInjectServiceHostFactory();
            var serviceType = typeof(ServiceWithoutInterface);
            ExceptionAssert.Throws<NotSupportedException>(() => lightInjectServiceHostFactory.CreateServiceHost(serviceType.AssemblyQualifiedName, new Uri[] { }));
        }

        [TestMethod]
        public void CreateServiceHost_ServiceTypeIsInterfaceWithoutServiceContractAttribute_ThrowsNotSupportedException()
        {
            var lightInjectServiceHostFactory = new LightInjectServiceHostFactory();
            var serviceType = typeof(IServiceWithoutServiceContractAttribute);
            ExceptionAssert.Throws<NotSupportedException>(() => lightInjectServiceHostFactory.CreateServiceHost(serviceType.AssemblyQualifiedName, new Uri[] { }));
        }

        [TestMethod]
        public void CreateServiceHost_ServiceTypeIsNotKnown_ThrowsArgumentException()
        {
            var lightInjectServiceHostFactory = new LightInjectServiceHostFactory();
            ExceptionAssert.Throws<ArgumentException>(() => lightInjectServiceHostFactory.CreateServiceHost("unknown", new Uri[] { }));
        }      
    }       
}
