namespace LightInject.Tests
{
    public class TestBase
    {
        internal virtual IServiceContainer CreateContainer()
        {
            return ContainerFactory.CreateContainer();
        }        
    }
}