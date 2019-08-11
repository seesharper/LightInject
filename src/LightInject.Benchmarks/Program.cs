using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;

namespace LightInject.Microsoft.DependencyInjection.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<PerformanceTests>();
        }
    }

    public class PerformanceTests
    {
        private IServiceProvider lightInjectServiceProvider;

        private IServiceProvider microsoftServiceProvider;

        private IServiceContainer serviceContainer;

        private IServiceFactory scope;



        [GlobalSetup]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<Foo>();

            serviceContainer = new ServiceContainer();
            scope = serviceContainer.BeginScope();
            serviceContainer.Register<Foo>();
            //lightInjectServiceProvider = serviceContainer.CreateServiceProvider(serviceCollection);

            microsoftServiceProvider = serviceCollection.BuildServiceProvider();
        }

        [Benchmark]
        public void UsingLightInject()
        {
            //var instance = lightInjectServiceProvider.GetService<Foo>();

            var instance = scope.GetInstance<Foo>();

            //var factory = (IServiceScopeFactory)lightInjectServiceProvider.GetService(typeof(IServiceScopeFactory));

            // using (var scope = factory.CreateScope())
            // {
            //     //var controller = scope.ServiceProvider.GetService(typeof(Foo));
            // }
        }

        [Benchmark]
        public void UsingMicrosoft()
        {
            var instance = microsoftServiceProvider.GetService<Foo>();

            //var factory = (IServiceScopeFactory)microsoftServiceProvider.GetService(typeof(IServiceScopeFactory));

            // using (var scope = factory.CreateScope())
            // {
            //     //var controller = scope.ServiceProvider.GetService(typeof(Foo));
            // }
        }
    }


    public class Foo
    {

    }

}
