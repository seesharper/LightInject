using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace LightInject.Benchmarks
{
    [MemoryDiagnoser]
    public class PerRequestBenchmarks
    {
        private IServiceScope microsoftScope;

        private IServiceContainer serviceContainer;

        private IServiceFactory lightInjectScope;

        [GlobalSetup]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<DisposableFoo>();

            serviceContainer = new ServiceContainer(new ContainerOptions() { EnableCurrentScope = false });
            serviceContainer.Register<Foo>(new PerRequestLifeTime());

            lightInjectScope = serviceContainer.BeginScope();

            var microsoftServiceProvider = serviceCollection.BuildServiceProvider();
            microsoftScope = microsoftServiceProvider.CreateScope();
        }

        [Benchmark]
        public void UsingLightInject()
        {
            var instance = lightInjectScope.GetInstance<Foo>();
        }

        [Benchmark]
        public void UsingMicrosoft()
        {
            var instance = microsoftScope.ServiceProvider.GetService<Foo>();
        }
    }


}