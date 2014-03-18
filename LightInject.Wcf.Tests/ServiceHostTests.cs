namespace LightInject.Wcf.Tests
{
    using System;
    using LightInject.Interception;
    using LightInject.Tests;
    using LightInject.Wcf.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceHostTests : TestBase
    {
        [TestMethod]
        public void CreateServiceHost_ServiceTypeIsInterfaceWithServiceContractAttribute_ReturnsServieHostWithProxyAsServiceType()
        {
            using (var serviceHost = StartService<IService>())
            {
                Assert.IsTrue(typeof(IProxy).IsAssignableFrom(serviceHost.Description.ServiceType));
            }                                     
        }

        [TestMethod]
        public void CreateServiceHost_ServiceTypeIsConcrete_ThrowsNotSupportedException()
        {            
            ExceptionAssert.Throws<NotSupportedException>(() => StartService<ServiceWithoutInterface>());
        }

        [TestMethod]
        public void CreateServiceHost_ServiceTypeIsInterfaceWithoutServiceContractAttribute_ThrowsNotSupportedException()
        {            
            ExceptionAssert.Throws<NotSupportedException>(() => StartService<IServiceWithoutServiceContractAttribute>());
        }

        [TestMethod]
        public void CreateServiceHost_ConfiguredService_AppliesConfiguration()
        {
            using (var serviceHost = StartService<IConfiguredService>())
            {
                Assert.AreEqual(2, serviceHost.Description.Endpoints.Count);
            }                        
        }
    }       
}
