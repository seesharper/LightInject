using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class ServiceRegistryExtensionTests : TestBase
    {
        private const string ServiceName = "SomeFoo";

        [Fact]
        public void ShouldRegisterSingletonService()
        {
            var container = CreateContainer();

            container.RegisterSingleton<IFoo, Foo>();

            AssertSingletonRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedSingletonService()
        {
            var container = CreateContainer();

            container.RegisterSingleton<IFoo, Foo>(ServiceName);

            AssertSingletonRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterSingletonServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterSingleton(typeof(IFoo), typeof(Foo));

            AssertSingletonRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedSingletonServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterSingleton(typeof(IFoo), typeof(Foo), ServiceName);
            AssertSingletonRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterSingletonServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterSingleton<IFoo>(f => new Foo());

            AssertSingletonRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedSingletonServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterSingleton<IFoo>(f => new Foo(), ServiceName);

            AssertSingletonRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterSingletonServiceUsingNonGenericFactory()
        {
            var container = CreateContainer();

            container.RegisterSingleton(typeof(IFoo), f => new Foo());

            AssertSingletonRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedSingletonServiceUsingNonGenericFactory()
        {
            var container = CreateContainer();

            container.RegisterSingleton(typeof(IFoo), f => new Foo(), ServiceName);

            AssertSingletonRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterConcreteSingletonService()
        {
            var container = CreateContainer();

            container.RegisterSingleton<Foo>();

            AssertSingletonRegistration<Foo>(container);
        }

        [Fact]
        public void ShouldRegisterConcreteSingletonServiceUsingType()
        {
            var container = CreateContainer();

            container.RegisterSingleton(typeof(Foo));

            AssertSingletonRegistration<Foo>(container);
        }

        [Fact]
        public void ShouldRegisterScopedService()
        {
            var container = CreateContainer();

            container.RegisterScoped<IFoo, Foo>();

            AssertScopedRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedScopedService()
        {
            var container = CreateContainer();

            container.RegisterScoped<IFoo, Foo>(ServiceName);

            AssertScopedRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterScopedServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterScoped(typeof(IFoo), typeof(Foo));
            AssertScopedRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedScopedServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterScoped(typeof(IFoo), typeof(Foo), ServiceName);
            AssertScopedRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterScopedServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterScoped<IFoo>(f => new Foo());

            AssertScopedRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedScopedServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterScoped<IFoo>(f => new Foo(), ServiceName);

            AssertScopedRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterScopedServiceUsingNonGenericFactory()
        {
            var container = CreateContainer();

            container.RegisterScoped(typeof(IFoo), f => new Foo());

            AssertScopedRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedScopedServiceUsingonGenericFactory()
        {
            var container = CreateContainer();

            container.RegisterScoped(typeof(IFoo), f => new Foo(), ServiceName);

            AssertScopedRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterConcreteScopedService()
        {
            var container = CreateContainer();

            container.RegisterScoped<Foo>();

            AssertScopedRegistration<Foo>(container);
        }

        [Fact]
        public void ShouldRegisterConcreteScopedServiceUsingType()
        {
            var container = CreateContainer();

            container.RegisterScoped(typeof(Foo));

            AssertScopedRegistration<Foo>(container);
        }


        [Fact]
        public void ShouldRegisterTransientService()
        {
            var container = CreateContainer();

            container.RegisterTransient<IFoo, Foo>();

            AssertTransientRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedTransientService()
        {
            var container = CreateContainer();

            container.RegisterTransient<IFoo, Foo>(ServiceName);

            AssertTransientRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterTransientServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterTransient(typeof(IFoo), typeof(Foo));
            AssertTransientRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedTransientServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterTransient(typeof(IFoo), typeof(Foo), ServiceName);
            AssertTransientRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterTransientServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterTransient<IFoo>(f => new Foo());

            AssertTransientRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedTransientServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterTransient<IFoo>(f => new Foo(), ServiceName);

            AssertTransientRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterTransientServiceUsingNonGenericFactory()
        {
            var container = CreateContainer();

            container.RegisterTransient(typeof(IFoo), f => new Foo());

            AssertTransientRegistration<IFoo>(container);
        }

        [Fact]
        public void ShouldRegisterNamedTransientServiceUsingonGenericFactory()
        {
            var container = CreateContainer();

            container.RegisterTransient(typeof(IFoo), f => new Foo(), ServiceName);

            AssertTransientRegistration<IFoo>(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterConcreteTransientService()
        {
            var container = CreateContainer();

            container.RegisterTransient<Foo>();

            AssertTransientRegistration<Foo>(container);
        }

        [Fact]
        public void ShouldRegisterConcreteTransientServiceUsingType()
        {
            var container = CreateContainer();

            container.RegisterTransient(typeof(Foo));

            AssertTransientRegistration<Foo>(container);
        }

        [Fact]
        public void ShouldOverrideUsingImplementingType()
        {
            var container = CreateContainer();

            container.Register<IFoo, Foo>();
            container.Override<IFoo, AnotherFoo>();

            Assert.IsType<AnotherFoo>(container.GetInstance<IFoo>());
        }

        [Fact]
        public void ShouldOverrideUsingImplementingTypeWithLifetime()
        {
            var container = CreateContainer();

            container.Register<IFoo, Foo>();
            container.Override<IFoo, AnotherFoo>(new PerContainerLifetime());
            Assert.IsType<AnotherFoo>(container.GetInstance<IFoo>());
            var firstInstance = container.GetInstance<IFoo>();
            var secondInstance = container.GetInstance<IFoo>();

            Assert.Same(firstInstance, secondInstance);
        }

        private void AssertTransientRegistration<TService>(IServiceRegistry serviceRegistry, string serviceName = null)
        {
            serviceName = serviceName ?? string.Empty;
            Assert.Contains<ServiceRegistration>(serviceRegistry.AvailableServices, sr => sr.ServiceType == typeof(TService) && sr.Lifetime == null && sr.ServiceName == serviceName);
        }

        private void AssertScopedRegistration<TService>(IServiceRegistry serviceRegistry, string serviceName = null)
        {
            AssertLifetimeRegistration<PerScopeLifetime, TService>(serviceRegistry, serviceName);
        }

        private void AssertSingletonRegistration<TService>(IServiceRegistry serviceRegistry, string serviceName = null)
        {
            AssertLifetimeRegistration<PerContainerLifetime, TService>(serviceRegistry, serviceName);
        }

        private void AssertLifetimeRegistration<TLifetime, TService>(IServiceRegistry serviceRegistry, string serviceName = null)
        {
            serviceName = serviceName ?? string.Empty;
            Assert.Contains<ServiceRegistration>(serviceRegistry.AvailableServices, sr => sr.ServiceType == typeof(TService) && sr.Lifetime.GetType() == typeof(TLifetime) && sr.ServiceName == serviceName);
        }

        public class CompositionRootWithArgument : ICompositionRoot
        {
            public CompositionRootWithArgument(Configuration configuration)
            {
            }

            public void Compose(IServiceRegistry serviceRegistry)
            {
                serviceRegistry.Register<Foo>();
            }
        }

        public class Configuration
        {

        }
    }
}