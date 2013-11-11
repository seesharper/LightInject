namespace LightInject.Wcf.Tests
{
    using System;
    using System.ServiceModel;
    using LightInject.Wcf;
    using LightInject.Wcf.SampleLibrary;
    using LightInject.Wcf.SampleLibrary.Implementation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Important: Before running these test, the following command needs to be executed
    /// in a command prompt with administrator privileges.
    /// c:\netsh http add urlacl url=http://+:6000/ user=[username]
    /// </summary>
    [TestClass]
    public class InvocationTests
    {          
        [TestMethod]
        public void Invoke_ServiceWithoutDepdnency_IsInvoked()
        {                       
            var result = Invoke<IService, int>(c => c.Execute());
            Assert.AreEqual(42, result);            
        }

        [TestMethod]
        public void Invoke_ServiceWithSameDependencyTwice_CreatesScopedDependency()
        {
            Foo.InitializeCount = 0;
            Invoke<IServiceWithSameDependencyTwice, int>(c => c.Execute());
            Assert.AreEqual(1, Foo.InitializeCount);            
        }

        private ServiceHost StartService<TService>()
        {
            var serviceHost = new LightInjectServiceHostFactory().CreateServiceHost<TService>("http://localhost:6000");
            serviceHost.Open();
            return serviceHost;
        }

        private TResult Invoke<TService, TResult>(Func<TService, TResult> func)
        {
            using (StartService<TService>())
            {
                var calculatorFactory = new ChannelFactory<TService>(
                    new BasicHttpBinding(), new EndpointAddress("http://localhost:6000"));
                TService service = calculatorFactory.CreateChannel();
                var result = func(service);
                ((IClientChannel)service).Close();
                return result;
            }
        }

        

    }
}