namespace LightInject.Interception.Tests
{
    internal static class ContainerFactory
    {
        internal static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}