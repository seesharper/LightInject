namespace LightInject.Wcf.SampleLibrary.Implementation
{
    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IService, Service>();
            serviceRegistry.Register<IServiceWithSameDependencyTwice, ServiceWithSameDependencyTwice>();            
            serviceRegistry.Register<IFoo, Foo>(new PerScopeLifetime());
        }
    }
}