using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace LightInject.Benchmarks
{



    [MemoryDiagnoser]
    public class ControllerBenchmarks
    {
        private IServiceProvider defaultServiceProvider;

        private IServiceContainer serviceContainer;

        [GlobalSetup]
        public void Setup()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<TestController1>();
            serviceCollection.AddTransient<TestController2>();
            serviceCollection.AddTransient<TestController3>();
            serviceCollection.AddTransient<IRepositoryTransient1, RepositoryTransient1>();
            serviceCollection.AddTransient<IRepositoryTransient2, RepositoryTransient2>();
            serviceCollection.AddTransient<IRepositoryTransient3, RepositoryTransient3>();
            serviceCollection.AddTransient<IRepositoryTransient4, RepositoryTransient4>();
            serviceCollection.AddTransient<IRepositoryTransient5, RepositoryTransient5>();
            serviceCollection.AddScoped<IScopedService1, ScopedService1>();
            serviceCollection.AddScoped<IScopedService2, ScopedService2>();
            serviceCollection.AddScoped<IScopedService3, ScopedService3>();
            serviceCollection.AddScoped<IScopedService4, ScopedService4>();
            serviceCollection.AddScoped<IScopedService5, ScopedService5>();
            serviceCollection.AddSingleton<ISingleton1, Singleton1>();
            serviceCollection.AddSingleton<ISingleton2, Singleton2>();
            serviceCollection.AddSingleton<ISingleton3, Singleton3>();
            //lightInjectServiceProvider = serviceCollection.CreateLightInjectServiceProvider(new ContainerOptions() { EnableCurrentScope = false });
            //lightInjectServiceProvider = serviceCollection.CreateLightInjectServiceProvider();
            defaultServiceProvider = serviceCollection.BuildServiceProvider();

            serviceContainer = new ServiceContainer(new ContainerOptions() { EnableCurrentScope = false });
            serviceContainer.RegisterTransient<TestController1>();
            serviceContainer.RegisterTransient<TestController2>();
            serviceContainer.RegisterTransient<TestController3>();
            serviceContainer.RegisterTransient<IRepositoryTransient1, RepositoryTransient1>();
            serviceContainer.RegisterTransient<IRepositoryTransient2, RepositoryTransient2>();
            serviceContainer.RegisterTransient<IRepositoryTransient3, RepositoryTransient3>();
            serviceContainer.RegisterTransient<IRepositoryTransient4, RepositoryTransient4>();
            serviceContainer.RegisterTransient<IRepositoryTransient5, RepositoryTransient5>();
            serviceContainer.RegisterScoped<IScopedService1, ScopedService1>();
            serviceContainer.RegisterScoped<IScopedService2, ScopedService2>();
            serviceContainer.RegisterScoped<IScopedService3, ScopedService3>();
            serviceContainer.RegisterScoped<IScopedService4, ScopedService4>();
            serviceContainer.RegisterScoped<IScopedService5, ScopedService5>();
            serviceContainer.RegisterSingleton<ISingleton1, Singleton1>();
            serviceContainer.RegisterSingleton<ISingleton2, Singleton2>();
            serviceContainer.RegisterSingleton<ISingleton3, Singleton3>();
            serviceContainer.Compile();
        }

        [Benchmark]
        public void UsingLightInject()
        {
            using (var scope = serviceContainer.BeginScope())
            {
                var controller = scope.GetInstance<IRepositoryTransient1>();
            }
        }

        [Benchmark]
        public void UsingMicrosoft()
        {
            using (var scope = defaultServiceProvider.CreateScope())
            {
                var controller = scope.ServiceProvider.GetService<IRepositoryTransient1>();
            }
        }


    }
}