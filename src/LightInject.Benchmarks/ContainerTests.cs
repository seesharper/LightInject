namespace LightInject.BenchMarks
{
    using DryIoc;
    using Grace.DependencyInjection;
    using Grace.Dynamic;

    internal class ContainerTests
    {
        protected readonly ServiceContainer LightInjectContainer = new ServiceContainer();
        protected readonly DryIoc.Container DryIocContainer = new Container();
        protected readonly Grace.DependencyInjection.DependencyInjectionContainer GraceContainer =
            new DependencyInjectionContainer(GraceDynamicMethod.Configuration());

        public ContainerTests()
        {
            LightInjectContainer
                .Register<ITransient1, Transient1>()
                .Register<ITransient2, Transient2>()
                .Register<ITransient3, Transient3>()
                .Register<ISingleton1, Singleton1>(new PerContainerLifetime())
                .Register<ISingleton2, Singleton2>(new PerContainerLifetime())
                .Register<ISingleton3, Singleton3>(new PerContainerLifetime());


            DryIocContainer.Register<ITransient1, Transient1>();
            DryIocContainer.Register<ITransient2, Transient2>();
            DryIocContainer.Register<ITransient3, Transient3>();
            DryIocContainer.Register<ISingleton1, Singleton1>();
            DryIocContainer.Register<ISingleton2, Singleton2>();
            DryIocContainer.Register<ISingleton3, Singleton3>();


            GraceContainer.Configure(ioc =>
            {
                ioc.Export<Transient1>().As<ITransient1>();
                ioc.Export<Transient2>().As<ITransient2>();
                ioc.Export<Transient3>().As<ITransient3>();
                ioc.Export<Singleton1>().As<ISingleton1>().Lifestyle.Singleton();
                ioc.Export<Singleton2>().As<ISingleton2>().Lifestyle.Singleton();
                ioc.Export<Singleton3>().As<ISingleton3>().Lifestyle.Singleton();
            });
        }
    }
}