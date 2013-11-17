namespace LightInject.Wcf.Tests
{
    using System;
    using System.ServiceModel;

    using LightInject.Interception;
    using LightInject.Tests;
    using LightInject.Wcf.Client;
    using LightInject.Wcf.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BindingProviderTests
    {
        private IServiceContainer serviceContainer;

        [TestInitialize]
        public void TestInitialize()
        {
            serviceContainer = new ServiceContainer();
            serviceContainer.EnableWcf();
        }

        [TestMethod]
        public void GetInstance_RequestedTwice_ReturnsSameInstance()
        {
            var firstProvider = serviceContainer.GetInstance<IUriProvider>();
            var secondProvider = serviceContainer.GetInstance<IUriProvider>();
            Assert.AreSame(firstProvider, secondProvider);
        }

        [TestMethod]
        public void GetBinding_Http_ReturnsBasicHttpBinding()
        {
            serviceContainer.Intercept(m => m.IsDeclaredBy<IUriProvider>(), info => new Uri("http://tempuri.org"));
            var provider = serviceContainer.GetInstance<IBindingProvider>();
            

            var binding = provider.GetBinding(typeof(IService));

            Assert.IsInstanceOfType(binding, typeof(BasicHttpBinding));
        }

        [TestMethod]
        public void GetBinding_Https_ReturnsWsHttpBinding()
        {
            serviceContainer.Intercept(m => m.IsDeclaredBy<IUriProvider>(), info => new Uri("https://tempuri.org"));
            var provider = serviceContainer.GetInstance<IBindingProvider>();
            

            var binding = provider.GetBinding(typeof(IService));

            Assert.IsInstanceOfType(binding, typeof(WSHttpBinding));
        }

        [TestMethod]
        public void GetBinding_NetTcp_ReturnsNetTcpBinding()
        {
            serviceContainer.Intercept(m => m.IsDeclaredBy<IUriProvider>(), info => new Uri("net.tcp://tempuri.org"));
            var provider = serviceContainer.GetInstance<IBindingProvider>();


            var binding = provider.GetBinding(typeof(IService));

            Assert.IsInstanceOfType(binding, typeof(NetTcpBinding));
        }

        [TestMethod]
        public void GetBinding_NetPipe_ReturnsNetNamedPipeBinding()
        {
            serviceContainer.Intercept(m => m.IsDeclaredBy<IUriProvider>(), info => new Uri("net.pipe://tempuri.org"));
            var provider = serviceContainer.GetInstance<IBindingProvider>();


            var binding = provider.GetBinding(typeof(IService));

            Assert.IsInstanceOfType(binding, typeof(NetNamedPipeBinding));
        }

        [TestMethod]
        public void GetBinding_UnknownSheme_ThrowsException()
        {
            serviceContainer.Intercept(m => m.IsDeclaredBy<IUriProvider>(), info => new Uri("unknown://tempuri.org"));
            var provider = serviceContainer.GetInstance<IBindingProvider>();

            ExceptionAssert.Throws<InvalidOperationException>(() => provider.GetBinding(typeof(IService)), e => e.Message.Contains("Unable to resolve binding"));
        }
    }
}