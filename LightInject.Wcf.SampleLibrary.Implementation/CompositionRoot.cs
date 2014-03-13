namespace LightInject.Wcf.SampleLibrary.Implementation
{
    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IService, Service>();
            serviceRegistry.Register<IServiceWithSameDependencyTwice, ServiceWithSameDependencyTwice>();
            serviceRegistry.Register<IPerCallInstanceAndSingleConcurrency, PerCallInstanceAndSingleConcurrency>();
            serviceRegistry.Register<IPerCallInstanceAndMultipleConcurrency, PerCallInstanceAndMultipleConcurrency>();
            serviceRegistry.Register<IPerCallInstanceAndReentrantConcurrency, PerCallInstanceAndReentrantConcurrency>();            
            serviceRegistry.Register<IPerSessionInstanceAndSingleConcurrency, PerSessionInstanceAndSingleConcurrency>();
            serviceRegistry.Register<IPerSessionInstanceAndMultipleConcurrency, PerSessionInstanceAndMultipleConcurrency>();
            serviceRegistry.Register<IPerSessionInstanceAndReentrantConcurrency, PerSessionInstanceAndReentrantConcurrency>();
            serviceRegistry.Register<ISingleInstanceAndSingleConcurrency, SingleInstanceAndSingleConcurrency>();
            serviceRegistry.Register<ISingleInstanceAndMultipleConcurrency, SingleInstanceAndMultipleConcurrency>();
            serviceRegistry.Register<ISingleInstanceAndReentrantConcurrency, SingleInstanceAndReentrantConcurrency>();
            serviceRegistry.Register<IAsyncService, AsyncService>();
            serviceRegistry.Register<IFoo, Foo>(new PerScopeLifetime());
        }
    }
}