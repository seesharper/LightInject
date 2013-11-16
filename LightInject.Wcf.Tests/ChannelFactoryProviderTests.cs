namespace LightInject.Wcf.Tests
{
    using System.ServiceModel;
   
    using LightInject.Wcf.Client;
    using LightInject.Wcf.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
   
    [TestClass]
    public class ChannelFactoryProviderTests
    {
        private static readonly IServiceContainer ServiceContainer = new ServiceContainer();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ServiceContainer.EnableWcf();
        }

        [TestMethod]
        public void GetChannelFactory_Interface_ReturnsChannelFactory()
        {
            var provider = ServiceContainer.GetInstance<IChannelFactoryProvider>();
            ChannelFactory<IService> channelFactory = provider.GetChannelFactory<IService>();                     
            Assert.IsNotNull(channelFactory);
        }

        [TestMethod]
        public void GetChannelFactory_Interface_ReturnsChannelFactoryAsSingleton()
        {
            var provider = ServiceContainer.GetInstance<IChannelFactoryProvider>();
            ChannelFactory<IService> firstFactory = provider.GetChannelFactory<IService>();
            ChannelFactory<IService> secondFactory = provider.GetChannelFactory<IService>();
            Assert.AreSame(firstFactory, secondFactory);
        }
    }
}