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
            var serviceHost = lightInjectServiceHostFactory.CreateServiceHost<IServiceWithServiceContractAttribute>();
            var returnedServiceType = serviceHost.Description.ServiceType;
            Assert.IsTrue(typeof(IProxy).IsAssignableFrom(returnedServiceType));
        }

        [TestMethod]
        public void CreateServiceHost_ServiceTypeIsConcrete_ThrowsNotSupportedException()
        {
            var lightInjectServiceHostFactory = new LightInjectServiceHostFactory();            
            ExceptionAssert.Throws<NotSupportedException>(() => lightInjectServiceHostFactory.CreateServiceHost<ServiceWithoutInterface>());
        }

        [TestMethod]
        public void CreateServiceHost_ServiceTypeIsInterfaceWithoutServiceContractAttribute_ThrowsNotSupportedException()
        {
            var lightInjectServiceHostFactory = new LightInjectServiceHostFactory();            
            ExceptionAssert.Throws<NotSupportedException>(() => lightInjectServiceHostFactory.CreateServiceHost<IServiceWithoutServiceContractAttribute>());
        }     
    }       
}
