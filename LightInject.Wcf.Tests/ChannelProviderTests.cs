namespace LightInject.Wcf.Tests
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using LightInject.Wcf.Client;
    using LightInject.Wcf.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ChannelProviderTests : TestBase
    {
        private IServiceContainer serviceContainer;

        [TestInitialize]
        public void TestInitialize()
        {
            serviceContainer = new ServiceContainer();
            serviceContainer.EnableWcf();
        }

        [TestMethod]
        public void GetInstance_ChannelProviderRequestedTwice_ReturnsSameInstance()
        {
            var firstProvider = serviceContainer.GetInstance<IChannelProvider>();
            var secondProvider = serviceContainer.GetInstance<IChannelProvider>();            
            
            Assert.AreSame(firstProvider, secondProvider);
        }

        [TestMethod]
        public void GetChannel_Service_ReturnsChannel()
        {
            var provider = serviceContainer.GetInstance<IChannelProvider>();
            
            IService channel = provider.GetChannel<IService>();
            
            Assert.IsInstanceOfType(channel, typeof(IChannel));
        }

        [TestMethod]
        public void GetChannel_RequestedTwice_ChannelsAreNotSame()
        {
            var provider = serviceContainer.GetInstance<IChannelProvider>();
            
            IService firstChannel = provider.GetChannel<IService>();
            IService secondChannel = provider.GetChannel<IService>();
            
            Assert.AreNotSame(firstChannel, secondChannel);
        }

        [TestMethod]
        public void Invoke_ExistingEndPoint_ReturnsResultFromService()
        {
            using (StartService<IService>())
            {
                var provider = serviceContainer.GetInstance<IChannelProvider>();
                IService channel = provider.GetChannel<IService>();
                
                var result = channel.Execute();

                Assert.AreEqual(42, result);
            }
        }

        [TestMethod]
        public void Invoke_OverTwoEndpoints_ReturnsResultFromService()
        {
            var provider = serviceContainer.GetInstance<IChannelProvider>();
            using (StartService<IService>())
            {                
                IService channel = provider.GetChannel<IService>();

                var result = channel.Execute();

                Assert.AreEqual(42, result);
            }

            using (StartService<IService>())
            {
                IService channel = provider.GetChannel<IService>();

                var result = channel.Execute();

                Assert.AreEqual(42, result);
            }
        }
    }
}