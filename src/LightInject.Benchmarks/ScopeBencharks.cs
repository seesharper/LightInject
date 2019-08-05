using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace LightInject.BenchMarks
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
            serviceCollection.AddScoped<Foo>();

            serviceContainer = new ServiceContainer();
            serviceContainer.RegisterScoped<Foo>();

            lightInjectScope = serviceContainer.BeginScope();

            var microsoftServiceProvider = serviceCollection.BuildServiceProvider();
            microsoftScope = microsoftServiceProvider.CreateScope();


            var simpleInjectorContainer = new Container();
            simpleInjectorContainer.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();


            simpleInjectorContainer.Register<Foo>(Lifestyle.Scoped);
            simpleInjectorScope = AsyncScopedLifestyle.BeginScope(simpleInjectorContainer);


        }

        [Benchmark]
        public void UsingLightInject()
        {
            var instance = lightInjectScope.GetInstance<Foo>();
        }

        [Benchmark]
        public void UsingSimpleInjector()
        {
            var instance = simpleInjectorScope.GetInstance<Foo>();
        }

        [Benchmark]
        public void UsingMicrosoft()
        {
#pragma warning disable IDE0059
            var instance = microsoftScope.ServiceProvider.GetService<Foo>();
        }
    }


    public class Foo : IDisposable
    {
        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}