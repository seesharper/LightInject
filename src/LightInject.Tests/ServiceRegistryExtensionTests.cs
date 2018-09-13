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
            
            AssertSingletonRegistration(container);
        }

        [Fact]
         public void ShouldRegisterNamedSingletonService()
        {
            var container = CreateContainer();

            container.RegisterSingleton<IFoo, Foo>(ServiceName);
            
            AssertSingletonRegistration(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterSingletonServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterSingleton(typeof(IFoo), typeof(Foo));
            AssertSingletonRegistration(container);
        }

        public void ShouldRegisterNamedSingletonServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterSingleton(typeof(IFoo), typeof(Foo), ServiceName);
            AssertSingletonRegistration(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterSingletonServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterSingleton<IFoo>(f => new Foo());
            
            AssertSingletonRegistration(container);
        }

        [Fact]
         public void ShouldRegisterNamedSingletonServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterSingleton<IFoo>(f => new Foo(), ServiceName);
            
            AssertSingletonRegistration(container, ServiceName);
        }


        [Fact]
        public void ShouldRegisterScopedService()
        {
            var container = CreateContainer();

            container.RegisterScoped<IFoo, Foo>();
            
            AssertScopedRegistration(container);
        }

        [Fact]
         public void ShouldRegisterNamedScopedService()
        {
            var container = CreateContainer();

            container.RegisterScoped<IFoo, Foo>(ServiceName);
            
            AssertScopedRegistration(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterScopedServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterScoped(typeof(IFoo), typeof(Foo));
            AssertScopedRegistration(container);
        }

        public void ShouldRegisterNamedScopedServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterScoped(typeof(IFoo), typeof(Foo), ServiceName);
            AssertScopedRegistration(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterScopedServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterScoped<IFoo>(f => new Foo());
            
            AssertScopedRegistration(container);
        }

        [Fact]
         public void ShouldRegisterNamedScopedServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterScoped<IFoo>(f => new Foo(), ServiceName);
            
            AssertScopedRegistration(container, ServiceName);
        }

         [Fact]
        public void ShouldRegisterTransientService()
        {
            var container = CreateContainer();

            container.RegisterTransient<IFoo, Foo>();
            
            AssertTransientRegistration(container);
        }

         [Fact]
         public void ShouldRegisterNamedTransientService()
        {
            var container = CreateContainer();

            container.RegisterTransient<IFoo, Foo>(ServiceName);
            
            AssertTransientRegistration(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterTransientServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterTransient(typeof(IFoo), typeof(Foo));
            AssertTransientRegistration(container);
        }

        public void ShouldRegisterNamedTransientServiceUsingTypes()
        {
            var container = CreateContainer();

            container.RegisterTransient(typeof(IFoo), typeof(Foo), ServiceName);
            AssertTransientRegistration(container, ServiceName);
        }

        [Fact]
        public void ShouldRegisterTransientServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterTransient<IFoo>(f => new Foo());
            
            AssertTransientRegistration(container);
        }

        [Fact]
         public void ShouldRegisterNamedTransientServiceUsingFactory()
        {
            var container = CreateContainer();

            container.RegisterTransient<IFoo>(f => new Foo(), ServiceName);
            
            AssertTransientRegistration(container, ServiceName);
        }

        private void AssertTransientRegistration(IServiceRegistry serviceRegistry, string serviceName = null)
        {
            serviceName = serviceName ?? string.Empty;
            Assert.Contains<ServiceRegistration>(serviceRegistry.AvailableServices, sr => sr.ServiceType == typeof(IFoo) && sr.Lifetime == null && sr.ServiceName == serviceName);
        }

        private void AssertScopedRegistration(IServiceRegistry serviceRegistry, string serviceName = null)
        {
            AssertLifetimeRegistration<PerScopeLifetime>(serviceRegistry, serviceName);
        }

         private void AssertSingletonRegistration(IServiceRegistry serviceRegistry, string serviceName = null)
        {
            AssertLifetimeRegistration<PerContainerLifetime>(serviceRegistry, serviceName);
        }

        private void AssertLifetimeRegistration<TLifetime>(IServiceRegistry serviceRegistry, string serviceName = null)
        {
            serviceName = serviceName ?? string.Empty;
            Assert.Contains<ServiceRegistration>(serviceRegistry.AvailableServices, sr => sr.ServiceType == typeof(IFoo) && sr.Lifetime.GetType() == typeof(TLifetime) && sr.ServiceName == serviceName);
        }
   }
}