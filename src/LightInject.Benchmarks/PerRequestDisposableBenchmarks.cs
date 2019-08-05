using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace LightInject.Benchmarks
{
    [SimpleJob(launchCount: 1, warmupCount: 1, targetCount: 3)]
    [MemoryDiagnoser]
    public class PerRequestDisposableBenchmarks
    {
        private IServiceScope microsoftScope;

        private IServiceContainer serviceContainer;

        private IServiceFactory lightInjectScope;

        [GlobalSetup]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<DisposableFoo>();

            serviceContainer = new ServiceContainer();
            serviceContainer.Register<DisposableFoo>(new PerRequestLifeTime());

            lightInjectScope = serviceContainer.BeginScope();

            var microsoftServiceProvider = serviceCollection.BuildServiceProvider();
            microsoftScope = microsoftServiceProvider.CreateScope();
        }

        [Benchmark]
        public void UsingLightInject()
        {
            var instance = lightInjectScope.GetInstance<DisposableFoo>();
        }

        [Benchmark]
        public void UsingMicrosoft()
        {
            var instance = microsoftScope.ServiceProvider.GetService<DisposableFoo>();
        }
    }


}