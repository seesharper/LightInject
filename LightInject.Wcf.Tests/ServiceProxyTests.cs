using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightInject.Wcf.Tests
{
    using System.ServiceModel;

    using LightInject.Interception;
    using LightInject.Wcf.Client;
    using LightInject.Wcf.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class ServiceProxyTests
    {
        private static readonly IServiceContainer ServiceContainer = new ServiceContainer();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ServiceContainer.EnableWcf();
        }
        
        [TestMethod]
        public void GetProxy_Service_ProxyImplementsServiceInterface()
        {
            var proxyFactory = ServiceContainer.GetInstance<IServiceProxyFactory>();
            var proxy = proxyFactory.CreateProxy(typeof(IService));
            Assert.IsInstanceOfType(proxy, typeof(IService));
        }

        [TestMethod]
        public void GetProxy_Service_CanInvokeService()
        {
            using (this.StartService<IService>())
            {
                var proxyFactory = ServiceContainer.GetInstance<IServiceProxyFactory>();
                var proxy = (IService)proxyFactory.CreateProxy(typeof(IService));
                var result = proxy.Execute();
                Assert.AreEqual(42, result);
            }
        }

        [TestMethod]
        public void GetProxy_Service_IsClosedAfterInvocation()
        {
            using (this.StartService<IService>())
            {
                //ServiceContainer.Intercept(sr => sr.ServiceType == typeof(IService), )
                
                
                var proxyFactory = ServiceContainer.GetInstance<IServiceProxyFactory>();
                var proxy = (IService)proxyFactory.CreateProxy(typeof(IService));
                var result = proxy.Execute();
                
            }
        }




        private ServiceHost StartService<TService>()
        {
            var serviceHost = new LightInjectServiceHostFactory().CreateServiceHost<TService>("http://localhost:6000/" + typeof(TService).FullName + ".svc");
            serviceHost.Open();
            return serviceHost;
        }
    }
}
