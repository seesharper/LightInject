using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using Xunit;

namespace LightInject.Tests;

public class KeyedMicrosoftTests : TestBase
{
    internal override IServiceContainer CreateContainer()
    {
        return new ServiceContainer(options =>
        {
            options.AllowMultipleRegistrations = true;
            options.EnableCurrentScope = false;
            options.OptimizeForLargeObjectGraphs = false;
        })
        {
            AssemblyScanner = new NoOpAssemblyScanner()
        };
    }

    [Fact]
    public void ResolveKeyedService()
    {
        var service1 = new Service();
        var service2 = new Service();
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        container.RegisterInstance<IService>(service1, "service1");
        container.RegisterInstance<IService>(service2, "service2");

        Assert.Null(container.TryGetInstance<IService>());
        Assert.Same(service1, container.GetInstance<IService>("service1"));
        Assert.Same(service2, container.GetInstance<IService>("service2"));
    }

    [Fact]
    public void ResolveNullKeyedService()
    {
        var service1 = new Service();
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        container.RegisterInstance<IService>(service1, null);

        var nonKeyed = container.TryGetInstance<IService>();
        var nullKey = container.TryGetInstance<IService>(null);

        Assert.Same(service1, nonKeyed);
        Assert.Same(service1, nullKey);
    }

    [Fact]
    public void ResolveNonKeyedService()
    {
        var service1 = new Service();
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        container.RegisterInstance<IService>(service1);

        var nonKeyed = container.GetInstance<IService>();
        var nullKey = container.GetInstance<IService>(null);

        Assert.Same(service1, nonKeyed);
        Assert.Same(service1, nullKey);
    }


    [Fact]
    public void ResolveKeyedOpenGenericService()
    {
        // Arrange
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        container.RegisterTransient(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>), "my-service");
        container.Register<IFakeSingletonService, FakeService>(new PerRootScopeLifetime(rootScope));

        // Act
        var genericService = rootScope.GetInstance<IFakeOpenGenericService<IFakeSingletonService>>("my-service");
        var singletonService = rootScope.GetInstance<IFakeSingletonService>();

        // Assert
        Assert.Same(singletonService, genericService.Value);
    }


    [Fact]
    public void ResolveKeyedServices()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var service1 = new Service();
        var service2 = new Service();
        var service3 = new Service();
        var service4 = new Service();

        container.RegisterInstance<IService>(service1, "first-service");
        container.RegisterInstance<IService>(service2, "service");
        container.RegisterInstance<IService>(service3, "service");
        container.RegisterInstance<IService>(service4, "service");

        // var serviceCollection = new ServiceCollection();
        // serviceCollection.AddKeyedSingleton<IService>("first-service", service1);
        // serviceCollection.AddKeyedSingleton<IService>("service", service2);
        // serviceCollection.AddKeyedSingleton<IService>("service", service3);
        // serviceCollection.AddKeyedSingleton<IService>("service", service4);

        // var provider = CreateServiceProvider(serviceCollection);

        var firstSvc = rootScope.GetInstance<IEnumerable<IService>>("first-service").ToList();
        Assert.Single(firstSvc);
        Assert.Same(service1, firstSvc[0]);

        var services = rootScope.GetInstance<IEnumerable<IService>>("service").ToList();
        Assert.Equal(new[] { service2, service3, service4 }, services);
    }

    [Fact]
    public void ResolveKeyedServicesAnyKey()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();


        var service1 = new Service();
        var service2 = new Service();
        var service3 = new Service();
        var service4 = new Service();
        var service5 = new Service();
        var service6 = new Service();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IService>("first-service", service1);
        serviceCollection.AddKeyedSingleton<IService>("service", service2);
        serviceCollection.AddKeyedSingleton<IService>("service", service3);
        serviceCollection.AddKeyedSingleton<IService>("service", service4);
        serviceCollection.AddKeyedSingleton<IService>(null, service5);
        serviceCollection.AddSingleton<IService>(service6);

        container.RegisterInstance<IService>(service1, "first-service");
        container.RegisterInstance<IService>(service2, "service");
        container.RegisterInstance<IService>(service3, "service");
        container.RegisterInstance<IService>(service4, "service");
        container.RegisterInstance<IService>(service5, null);
        container.RegisterInstance<IService>(service6);

        var test = KeyedService.AnyKey.ToString();
        // var provider = CreateServiceProvider(serviceCollection);

        // // Return all services registered with a non null key
        // //var allServices = provider.GetKeyedServices<IService>(KeyedService.AnyKey).ToList();
        var allServices = rootScope.GetInstance<IEnumerable<IService>>(KeyedService.AnyKey.ToString()).ToList();
        Assert.Equal(4, allServices.Count);
        Assert.Equal(new[] { service1, service2, service3, service4 }, allServices);

        // Check again (caching)
        var allServices2 = rootScope.GetInstance<IEnumerable<IService>>(KeyedService.AnyKey.ToString()).ToList();
        Assert.Equal(allServices, allServices2);
    }

    [Fact]
    public void ResolveKeyedServicesAnyKeyWithAnyKeyRegistration()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var service1 = new Service();
        var service2 = new Service();
        var service3 = new Service();
        var service4 = new Service();
        var service5 = new Service();
        var service6 = new Service();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedTransient<IService>(KeyedService.AnyKey, (sp, key) => new Service());
        serviceCollection.AddKeyedSingleton<IService>("first-service", service1);
        serviceCollection.AddKeyedSingleton<IService>("service", service2);
        serviceCollection.AddKeyedSingleton<IService>("service", service3);
        serviceCollection.AddKeyedSingleton<IService>("service", service4);
        serviceCollection.AddKeyedSingleton<IService>(null, service5);
        serviceCollection.AddSingleton<IService>(service6);

        container.RegisterTransient<IService>((factory) => new Service(), KeyedService.AnyKey.ToString());
        container.RegisterInstance<IService>(service1, "first-service");
        container.RegisterInstance<IService>(service2, "service");
        container.RegisterInstance<IService>(service3, "service");
        container.RegisterInstance<IService>(service4, "service");
        container.RegisterInstance<IService>(service5, null);
        container.RegisterInstance<IService>(service6);


        //var provider = CreateServiceProvider(serviceCollection);


        // _ = provider.GetKeyedService<IService>("something-else");
        // _ = provider.GetKeyedService<IService>("something-else-again");

        container.TryGetInstance<IService>("something-else");
        container.TryGetInstance<IService>("something-else-again");

        // Return all services registered with a non null key, but not the one "created" with KeyedService.AnyKey
        var allServices = rootScope.GetInstance<IEnumerable<IService>>(KeyedService.AnyKey.ToString()).ToList();
        Assert.Equal(5, allServices.Count);
        Assert.Equal(new[] { service1, service2, service3, service4 }, allServices.Skip(1));
    }

    [Fact]
    public void ResolveKeyedGenericServices()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var service1 = new FakeService();
        var service2 = new FakeService();
        var service3 = new FakeService();
        var service4 = new FakeService();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IFakeOpenGenericService<PocoClass>>("first-service", service1);
        serviceCollection.AddKeyedSingleton<IFakeOpenGenericService<PocoClass>>("service", service2);
        serviceCollection.AddKeyedSingleton<IFakeOpenGenericService<PocoClass>>("service", service3);
        serviceCollection.AddKeyedSingleton<IFakeOpenGenericService<PocoClass>>("service", service4);

        container.RegisterInstance<IFakeOpenGenericService<PocoClass>>(service1, "first-service");
        container.RegisterInstance<IFakeOpenGenericService<PocoClass>>(service2, "service");
        container.RegisterInstance<IFakeOpenGenericService<PocoClass>>(service3, "service");
        container.RegisterInstance<IFakeOpenGenericService<PocoClass>>(service4, "service");



        //var provider = CreateServiceProvider(serviceCollection);

        //var firstSvc = provider.GetKeyedServices<IFakeOpenGenericService<PocoClass>>("first-service").ToList();
        var firstSvc = rootScope.GetInstance<IEnumerable<IFakeOpenGenericService<PocoClass>>>("first-service").ToList();
        Assert.Single(firstSvc);
        Assert.Same(service1, firstSvc[0]);

        var services = rootScope.GetInstance<IEnumerable<IFakeOpenGenericService<PocoClass>>>("service").ToList();
        Assert.Equal(new[] { service2, service3, service4 }, services);
    }

    [Fact]
    public void ResolveKeyedServiceSingletonInstance()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var service = new Service();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IService>("service1", service);
        container.RegisterInstance<IService>(service, "service1");




        Assert.Null(rootScope.TryGetInstance<IService>());
        Assert.Same(service, rootScope.GetInstance<IService>("service1"));
        Assert.Same(service, rootScope.GetInstance(typeof(IService), "service1"));
    }

    [Fact]
    public void ResolveKeyedServiceSingletonInstanceWithKeyInjection()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceKey = "this-is-my-service";
        var serviceCollection = new ServiceCollection();


        serviceCollection.AddKeyedSingleton<IService, Service>(serviceKey);

        // Note: This has to be solved in the adapter layer
        // Alternative is to look for the ServiceKeyAttribute rewrite the registration
        // using a function factory.
        //container.RegisterSingleton<IService, Service>(serviceKey);
        container.RegisterSingleton<IService>((factory) => new Service(serviceKey), serviceKey);

        // var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        var svc = rootScope.GetInstance<IService>(serviceKey);
        Assert.NotNull(svc);
        Assert.Equal(serviceKey, svc.ToString());
    }

    [Fact]
    public void ResolveKeyedServiceSingletonInstanceWithAnyKey()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IService, Service>(KeyedService.AnyKey);
        container.Register<IService, Service>(KeyedService.AnyKey.ToString(), new PerRootScopeLifetime(rootScope));

        Assert.Null(rootScope.TryGetInstance<IService>());

        var serviceKey1 = "some-key";
        var svc1 = rootScope.GetInstance<IService>(serviceKey1);
        Assert.NotNull(svc1);
        Assert.Equal(serviceKey1, svc1.ToString());

        var serviceKey2 = "some-other-key";
        var svc2 = rootScope.GetInstance<IService>(serviceKey2);
        Assert.NotNull(svc2);
        Assert.Equal(serviceKey2, svc2.ToString());
    }

    [Fact]
    public void ResolveKeyedServicesSingletonInstanceWithAnyKey()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        var service1 = new FakeService();
        var service2 = new FakeService();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IFakeOpenGenericService<PocoClass>>(KeyedService.AnyKey, service1);
        serviceCollection.AddKeyedSingleton<IFakeOpenGenericService<PocoClass>>("some-key", service2);
        container.Register<IFakeOpenGenericService<PocoClass>>(sf => service1, KeyedService.AnyKey.ToString(), new PerRootScopeLifetime(rootScope));
        container.Register<IFakeOpenGenericService<PocoClass>>(sf => service2, "some-key", new PerRootScopeLifetime(rootScope));

        // container.RegisterInstance<IFakeOpenGenericService<PocoClass>>(service1, KeyedService.AnyKey.ToString());
        // container.RegisterInstance<IFakeOpenGenericService<PocoClass>>(service2, "some-key");


        // var provider = CreateServiceProvider(serviceCollection);
        var services = rootScope.GetInstance<IEnumerable<IFakeOpenGenericService<PocoClass>>>("some-key").ToList();
        // var services = provider.GetKeyedServices<IFakeOpenGenericService<PocoClass>>("some-key").ToList();
        Assert.Equal(new[] { service1, service2 }, services);
    }


    internal interface IService { }

    internal class Service : IService
    {
        private readonly string _id;

        public Service() => _id = Guid.NewGuid().ToString();

        public Service([ServiceKey] string id) => _id = id;

        public override string? ToString() => _id;
    }

    internal class NoOpAssemblyScanner : IAssemblyScanner
    {
        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry, Func<ILifetime> lifetime, Func<Type, Type, bool> shouldRegister, Func<Type, Type, string> serviceNameProvider)
        {

        }

        public void Scan(Assembly assembly, IServiceRegistry serviceRegistry)
        {

        }
    }
}

