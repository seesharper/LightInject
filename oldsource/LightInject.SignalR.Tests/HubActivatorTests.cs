namespace LightInject.SignalR.Tests
{
    using LightInject.Interception;

    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HubActivatorTests
    {
        [TestMethod]
        public void GetInstance_HubActivator_ReturnsLightInjectHubActivator()
        {
            var container = new ServiceContainer();            
            var resolver = new LightInjectDependencyResolver(container);

            var instance = resolver.GetService(typeof(IHubActivator));

            Assert.IsInstanceOfType(instance, typeof(LightInjectHubActivator));
        }
                
        [TestMethod]
        public void Create_Hub_ReturnsHubProxy()
        {                                    
            var container = new ServiceContainer();
            container.EnableSignalR();
            container.Register<SampleHub>(new PerScopeLifetime());
            var activator = new LightInjectHubActivator(container);                     
            var hubDescriptor = new HubDescriptor { HubType = typeof(SampleHub) };
            
            var hub = activator.Create(hubDescriptor);

            Assert.IsInstanceOfType(hub, typeof(IProxy));
        }

        [TestMethod]
        public void Create_Hub_StartsScope()
        {
            var container = new ServiceContainer();
            container.EnableSignalR();
            container.Register<SampleHub>();
            var activator = new LightInjectHubActivator(container);
            var hubDescriptor = new HubDescriptor { HubType = typeof(SampleHub) };
            using (activator.Create(hubDescriptor))
            {
                Assert.IsNotNull(container.ScopeManagerProvider.GetScopeManager().CurrentScope);
            }                       
        }

        [TestMethod]
        public void Dispose_Hub_ClosesScope()
        {
            var container = new ServiceContainer();
            container.EnableSignalR();
            container.Register<SampleHub>();
            var activator = new LightInjectHubActivator(container);
            var hubDescriptor = new HubDescriptor { HubType = typeof(SampleHub) };

            var hub = activator.Create(hubDescriptor);

            hub.Dispose();

            Assert.IsNull(container.ScopeManagerProvider.GetScopeManager().CurrentScope);
        }
    }
}