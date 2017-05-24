namespace LightInject.Tests
{
    public class TestBase
    {
        internal virtual IServiceContainer CreateContainer()
        {
            return ContainerFactory.CreateContainer();
        }

        internal virtual IServiceContainer CreateContainer(ContainerOptions options)
        {
            return ContainerFactory.CreateContainer(options);
        }        
    }
}