using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace LightInject.Benchmarks
{
    [MemoryDiagnoser]
    public class ScopeBenchmarks
    {
        private IServiceScope microsoftScope;

        private IServiceContainer serviceContainer;

        private IServiceFactory lightInjectScope;

        private SimpleInjector.Scope simpleInjectorScope;


        [GlobalSetup]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<DisposableFoo>();

            serviceContainer = new ServiceContainer(new ContainerOptions() { EnableCurrentScope = false });
            serviceContainer.RegisterScoped<DisposableFoo>();

            lightInjectScope = serviceContainer.BeginScope();

            var microsoftServiceProvider = serviceCollection.BuildServiceProvider();
            microsoftScope = microsoftServiceProvider.CreateScope();


            var simpleInjectorContainer = new Container();
            simpleInjectorContainer.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();


            simpleInjectorContainer.Register<DisposableFoo>(Lifestyle.Scoped);
            simpleInjectorScope = AsyncScopedLifestyle.BeginScope(simpleInjectorContainer);


        }

        [Benchmark]
        public void UsingLightInject()
        {
            var instance = lightInjectScope.GetInstance<DisposableFoo>();
        }

        [Benchmark]
        public void UsingSimpleInjector()
        {
            var instance = simpleInjectorScope.GetInstance<DisposableFoo>();
        }

        [Benchmark]
        public void UsingMicrosoft()
        {
#pragma warning disable IDE0059
            var instance = microsoftScope.ServiceProvider.GetService<DisposableFoo>();
        }
    }


    public class DisposableFoo : IDisposable
    {
        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }

    public class Foo
    {

    }

}