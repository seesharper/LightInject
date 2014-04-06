using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightInject.Wcf.Tests
{
    using System.ServiceModel;    
    using LightInject.Wcf.Client;
    using LightInject.Wcf.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class ServiceProxyTests : TestBase
    {
        private IServiceContainer serviceContainer;
        
        [TestInitialize]
        public void TestInitialize()
        {
            serviceContainer = new ServiceContainer();
            serviceContainer.EnableWcfClient(new UriProvider("http://localhost:6000/"));
        }
                
        [TestMethod]
        public void GetProxy_Service_ProxyImplementsServiceInterface()
        {
            var proxyFactory = serviceContainer.GetInstance<IServiceProxyFactory>();
            var proxy = proxyFactory.CreateProxy(typeof(IService));
            Assert.IsInstanceOfType(proxy, typeof(IService));
        }

        [TestMethod]
        public void GetProxy_Service_CanInvokeService()
        {
            using (StartService<IService>())
            {
                var proxyFactory = serviceContainer.GetInstance<IServiceProxyFactory>();
                var proxy = (IService)proxyFactory.CreateProxy(typeof(IService));
                var result = proxy.Execute();
                Assert.AreEqual(42, result);
            }
        }
 
        [TestMethod]
        public void GetProxy_Service_IsClosedAfterInvocation()
        {
            using (StartService<IService>())
            {
                ICommunicationObject communicationObject = null;
                serviceContainer.Intercept(
                    m => m.Name == "GetChannel",
                    info =>
                        {
                            communicationObject = (ICommunicationObject)info.Proceed();
                            return communicationObject;
                        });
                                
                var proxyFactory = serviceContainer.GetInstance<IServiceProxyFactory>();
                var proxy = (IService)proxyFactory.CreateProxy(typeof(IService));
                proxy.Execute();
                Assert.IsTrue(communicationObject.State == CommunicationState.Closed);                
            }
        }                     
    }
}
