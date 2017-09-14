using Xunit;

namespace LightInject.Tests
{
    public class Issue349
    {
        [Fact]
        public void ShouldHandleDisposeWhenServiceFactoryIsRegistered()
        {
            var container = new ServiceContainer();
            container.RegisterInstance<IServiceFactory>(container);
            container.GetInstance<IServiceFactory>();
            container.Dispose();
        }

        [Fact]
        public void ShouldHandleDisposeWhenServiceContainerIsRegistered()
        {
            var container = new ServiceContainer();
            container.RegisterInstance<IServiceContainer>(container);
            container.GetInstance<IServiceContainer>();
            container.Dispose();
        }
    }
}