namespace LightInject.Wcf.Tests
{
    
    using System.ServiceModel;
    using System.ServiceModel.Description;    
    using LightInject.Wcf.Client;
    using LightInject.Wcf.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
   
    [TestClass]
    public class ChannelFactoryProviderTests : TestBase
    {
        private IServiceContainer serviceContainer;

        [TestInitialize]
        public void TestInitialize()
        {
            serviceContainer = new ServiceContainer();
            serviceContainer.EnableWcfClient(new UriProvider("http://localhost:6000/"));
        }

        [TestMethod]
        public void GetInstance_ChannelFactoryProviderRequestedTwice_ReturnsSameInstance()
        {
            var firstProvider = serviceContainer.GetInstance<IChannelFactoryProvider>();
            var secondProvider = serviceContainer.GetInstance<IChannelFactoryProvider>();
            Assert.AreSame(firstProvider, secondProvider);
        }

        [TestMethod]
        public void GetChannelFactory_Service_ReturnsChannelFactory()
        {
            var provider = serviceContainer.GetInstance<IChannelFactoryProvider>();
            ChannelFactory<IService> channelFactory = provider.GetChannelFactory<IService>();                     
            Assert.IsNotNull(channelFactory);
        }
        
        [TestMethod]
        public void GetChannelFactory_Twice_ReturnsSameChannelFactory()
        {
            var provider = serviceContainer.GetInstance<IChannelFactoryProvider>();
            ChannelFactory<IService> firstFactory = provider.GetChannelFactory<IService>();
            ChannelFactory<IService> secondFactory = provider.GetChannelFactory<IService>();
            Assert.AreSame(firstFactory, secondFactory);
        }

        [TestMethod]
        public void GetChannehlFactory_RegisteredEndPointBehavior_AppliesEndPointBehavior()
        {
            serviceContainer.Register<IEndpointBehavior, SampleEndPointBehavior>();
            var provider = serviceContainer.GetInstance<IChannelFactoryProvider>();
            ChannelFactory<IService> channelFactory = provider.GetChannelFactory<IService>();                     
            Assert.IsTrue(channelFactory.Endpoint.Behaviors.Contains(typeof(SampleEndPointBehavior)));
        }
    }
}