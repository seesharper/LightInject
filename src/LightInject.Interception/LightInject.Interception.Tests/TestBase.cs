namespace LightInject.Interception.Tests
{
    public class TestBase
    {
        internal virtual IServiceContainer CreateContainer()
        {
            return ContainerFactory.CreateContainer();
        }
    }
}