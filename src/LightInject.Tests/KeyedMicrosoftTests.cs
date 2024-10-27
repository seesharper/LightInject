using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightInject.SampleLibrary;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using Xunit;
using Xunit.Sdk;

namespace LightInject.Tests;

public class KeyedMicrosoftTests : TestBase
{
    internal override IServiceContainer CreateContainer()
    {
        var container = new ServiceContainer(options =>
        {
            options.AllowMultipleRegistrations = true;
            options.EnableCurrentScope = false;
            options.OptimizeForLargeObjectGraphs = false;
            options.EnableOptionalArguments = true;
        })
        {
            AssemblyScanner = new NoOpAssemblyScanner()
        };
        container.ConstructorDependencySelector = new AnnotatedConstructorDependencySelector();
        container.ConstructorSelector = new AnnotatedConstructorSelector(container.CanGetInstance);
        return container;
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

    [Fact]
    public void ResolveKeyedServiceSingletonInstanceWithKeyedParameter()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IService, Service>("service1");
        serviceCollection.AddKeyedSingleton<IService, Service>("service2");
        serviceCollection.AddSingleton<OtherService>();
        container.Register<IService, Service>("service1", new PerRootScopeLifetime(rootScope));
        container.Register<IService, Service>("service2", new PerRootScopeLifetime(rootScope));
        container.Register<OtherService>(new PerRootScopeLifetime(rootScope));


        //var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        var svc = rootScope.GetInstance<OtherService>();
        Assert.NotNull(svc);
        Assert.Equal("service1", svc.Service1.ToString());
        Assert.Equal("service2", svc.Service2.ToString());
    }


    [Fact]
    public void ResolveKeyedServiceWithKeyedParameter_MissingRegistration_SecondParameter()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();

        serviceCollection.AddKeyedSingleton<IService, Service>("service1");
        container.Register<IService, Service>("service1", new PerRootScopeLifetime(rootScope));
        // We are missing the registration for "service2" here and OtherService requires it.

        serviceCollection.AddSingleton<OtherService>();
        container.Register<OtherService>(new PerRootScopeLifetime(rootScope));

        // var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        Assert.Throws<InvalidOperationException>(() => rootScope.GetInstance<OtherService>());
    }

    [Fact]
    public void ResolveKeyedServiceWithKeyedParameter_MissingRegistration_FirstParameter()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();

        // We are not registering "service1" and "service1" keyed IService services and OtherService requires them.

        serviceCollection.AddSingleton<OtherService>();
        container.Register<OtherService>(new PerRootScopeLifetime(rootScope));

        // var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        Assert.Throws<InvalidOperationException>(() => rootScope.GetInstance<OtherService>());
    }

    [Fact]
    public void ResolveKeyedServiceWithKeyedParameter_MissingRegistrationButWithDefaults()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();

        // We are not registering "service1" and "service1" keyed IService services and OtherServiceWithDefaultCtorArgs
        // specifies them but has argument defaults if missing.

        serviceCollection.AddSingleton<OtherServiceWithDefaultCtorArgs>();
        container.Register<OtherServiceWithDefaultCtorArgs>(new PerRootScopeLifetime(rootScope));

        //var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        Assert.NotNull(rootScope.GetInstance<OtherServiceWithDefaultCtorArgs>());
    }

    [Fact]
    public void ResolveKeyedServiceWithKeyedParameter_MissingRegistrationButWithUnkeyedService()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();

        // We are not registering "service1" and "service1" keyed IService services and OtherService requires them,
        // but we are registering an unkeyed IService service which should not be injected into OtherService.
        serviceCollection.AddSingleton<IService, Service>();
        container.Register<IService, Service>(new PerRootScopeLifetime(rootScope));


        serviceCollection.AddSingleton<OtherService>();
        container.Register<OtherService>(new PerRootScopeLifetime(rootScope));

        // var provider = CreateServiceProvider(serviceCollection);

        Assert.NotNull(rootScope.GetInstance<IService>());
        Assert.Throws<InvalidOperationException>(() => rootScope.GetInstance<OtherService>());
    }

    [Fact]
    public void CreateServiceWithKeyedParameter()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();


        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IService, Service>();
        serviceCollection.AddKeyedSingleton<IService, Service>("service1");
        serviceCollection.AddKeyedSingleton<IService, Service>("service2");

        container.Register<IService, Service>(new PerRootScopeLifetime(rootScope));
        container.Register<IService, Service>("service1", new PerRootScopeLifetime(rootScope));
        container.Register<IService, Service>("service2", new PerRootScopeLifetime(rootScope));

        // var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<OtherService>());
        // var svc = ActivatorUtilities.CreateInstance<OtherService>(provider);
        // Assert.NotNull(svc);
        // Assert.Equal("service1", svc.Service1.ToString());
        // Assert.Equal("service2", svc.Service2.ToString());
    }



    //  [Fact]
    //     public void ResolveKeyedServiceSingletonFactory()
    //     {
    //         var service = new Service();
    //         var serviceCollection = new ServiceCollection();
    //         serviceCollection.AddKeyedSingleton<IService>("service1", (sp, key) => service);

    //         var provider = CreateServiceProvider(serviceCollection);

    //         Assert.Null(provider.GetService<IService>());
    //         Assert.Same(service, provider.GetKeyedService<IService>("service1"));
    //         Assert.Same(service, provider.GetKeyedService(typeof(IService), "service1"));
    //     }


    [Fact]
    public void ResolveKeyedServiceSingletonFactory()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var service = new Service();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IService>("service1", (sp, key) => service);
        container.Register<IService>((factory, key) => service, "service1", new PerRootScopeLifetime(rootScope));

        //var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        Assert.Same(service, rootScope.GetInstance<IService>("service1"));
        Assert.Same(service, rootScope.GetInstance(typeof(IService), "service1"));
    }

    [Fact]
    public void ResolveKeyedServiceSingletonFactoryWithAnyKey()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IService>(KeyedService.AnyKey, (sp, key) => new Service((string)key));
        container.Register<IService>((factory, key) => new Service(key), KeyedService.AnyKey.ToString(), new PerRootScopeLifetime(rootScope));

        Assert.Null(rootScope.TryGetInstance<IService>());

        for (int i = 0; i < 3; i++)
        {
            var key = "service" + i;
            var s1 = rootScope.GetInstance<IService>(key);
            var s2 = rootScope.GetInstance<IService>(key);
            Assert.Same(s1, s2);
            Assert.Equal(key, s1.ToString());
        }
    }

    [Fact]
    public void ResolveKeyedServiceSingletonFactoryWithAnyKeyIgnoreWrongType()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedTransient<IService, ServiceWithIntKey>(KeyedService.AnyKey);
        container.Register<IService, ServiceWithIntKey>(KeyedService.AnyKey.ToString());
        //var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        Assert.NotNull(rootScope.GetInstance<IService>(87.ToString()));
        Assert.ThrowsAny<InvalidOperationException>(() => rootScope.GetInstance<IService>(new object().ToString()));
        Assert.ThrowsAny<InvalidOperationException>(() => rootScope.GetInstance(typeof(IService), new object().ToString()));
    }

    [Fact]
    public void ResolveKeyedServiceSingletonType()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IService, Service>("service1");
        container.Register<IService, Service>("service1", new PerRootScopeLifetime(rootScope));

        // var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        Assert.Equal(typeof(Service), rootScope.GetInstance<IService>("service1")!.GetType());
    }

    [Fact]
    public void ResolveKeyedServiceTransientFactory()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedTransient<IService>("service1", (sp, key) => new Service(key as string));
        container.Register<IService>((factory, key) => new Service(key as string), "service1", null);

        //var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        var first = rootScope.GetInstance<IService>("service1");
        var second = rootScope.GetInstance<IService>("service1");
        Assert.NotSame(first, second);
        Assert.Equal("service1", first.ToString());
        Assert.Equal("service1", second.ToString());
    }

    [Fact]
    public void ResolveKeyedServiceTransientType()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedTransient<IService, Service>("service1");
        container.Register<IService, Service>("service1");

        //var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        var first = rootScope.GetInstance<IService>("service1");
        var second = rootScope.GetInstance<IService>("service1");
        Assert.NotSame(first, second);
    }

    [Fact]
    public void ResolveKeyedServiceTransientTypeWithAnyKey()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedTransient<IService, Service>(KeyedService.AnyKey);
        container.Register<IService, Service>(KeyedService.AnyKey.ToString());

        // var provider = CreateServiceProvider(serviceCollection);

        Assert.Null(rootScope.TryGetInstance<IService>());
        var first = rootScope.GetInstance<IService>("service1");
        var second = rootScope.GetInstance<IService>("service1");
        Assert.NotSame(first, second);
    }

    [Fact]
    public void ResolveKeyedSingletonFromInjectedServiceProvider()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IService, Service>("key");
        serviceCollection.AddSingleton<ServiceFactoryAccessor>();
        container.RegisterInstance<IServiceFactory>(container);
        container.Register<IService, Service>("key", new PerRootScopeLifetime(rootScope));
        container.Register<ServiceFactoryAccessor>(new PerRootScopeLifetime(rootScope));


        // var provider = CreateServiceProvider(serviceCollection);
        var accessor = rootScope.GetInstance<ServiceFactoryAccessor>();

        Assert.Null(accessor.ServiceFactory.TryGetInstance<IService>());

        var service1 = accessor.ServiceFactory.GetInstance<IService>("key");
        var service2 = accessor.ServiceFactory.GetInstance<IService>("key");

        Assert.Same(service1, service2);
    }

    [Fact]
    public void ResolveKeyedTransientFromInjectedServiceProvider()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedTransient<IService, Service>("key");
        // serviceCollection.AddSingleton<ServiceProviderAccessor>();
        container.RegisterInstance<IServiceFactory>(container);
        container.Register<IService, Service>("key");
        container.Register<ServiceFactoryAccessor>(new PerRootScopeLifetime(rootScope));

        // var provider = CreateServiceProvider(serviceCollection);
        var accessor = rootScope.GetInstance<ServiceFactoryAccessor>();

        Assert.Null(accessor.ServiceFactory.TryGetInstance<IService>());

        var service1 = accessor.ServiceFactory.GetInstance<IService>("key");
        var service2 = accessor.ServiceFactory.GetInstance<IService>("key");

        Assert.NotSame(service1, service2);
    }

    [Fact]
    public void ResolveKeyedSingletonFromScopeServiceProvider()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedSingleton<IService, Service>("key");
        container.Register<IService, Service>("key", new PerRootScopeLifetime(rootScope));

        // var provider = CreateServiceProvider(serviceCollection);
        // var scopeA = provider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        // var scopeB = provider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var scopeA = rootScope.BeginScope();
        var scopeB = rootScope.BeginScope();



        Assert.Null(scopeA.TryGetInstance<IService>());
        Assert.Null(scopeB.TryGetInstance<IService>());

        var serviceA1 = scopeA.GetInstance<IService>("key");
        var serviceA2 = scopeA.GetInstance<IService>("key");

        var serviceB1 = scopeB.GetInstance<IService>("key");
        var serviceB2 = scopeB.GetInstance<IService>("key");

        Assert.Same(serviceA1, serviceA2);
        Assert.Same(serviceB1, serviceB2);
        Assert.Same(serviceA1, serviceB1);
    }

    [Fact]
    public void ResolveKeyedScopedFromScopeServiceProvider()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();


        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedScoped<IService, Service>("key");
        container.RegisterScoped<IService, Service>("key");

        // var provider = CreateServiceProvider(serviceCollection);
        var scopeA = rootScope.BeginScope();
        var scopeB = rootScope.BeginScope();

        Assert.Null(scopeA.TryGetInstance<IService>());
        Assert.Null(scopeB.TryGetInstance<IService>());

        var serviceA1 = scopeA.GetInstance<IService>("key");
        var serviceA2 = scopeA.GetInstance<IService>("key");

        var serviceB1 = scopeB.GetInstance<IService>("key");
        var serviceB2 = scopeB.GetInstance<IService>("key");

        Assert.Same(serviceA1, serviceA2);
        Assert.Same(serviceB1, serviceB2);
        Assert.NotSame(serviceA1, serviceB1);
    }

    [Fact]
    public void ResolveKeyedTransientFromScopeServiceProvider()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();


        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedTransient<IService, Service>("key");
        container.Register<IService, Service>("key");


        // var provider = CreateServiceProvider(serviceCollection);
        var scopeA = rootScope.BeginScope();
        var scopeB = rootScope.BeginScope();

        Assert.Null(scopeA.TryGetInstance<IService>());
        Assert.Null(scopeB.TryGetInstance<IService>());

        var serviceA1 = scopeA.GetInstance<IService>("key");
        var serviceA2 = scopeA.GetInstance<IService>("key");

        var serviceB1 = scopeB.GetInstance<IService>("key");
        var serviceB2 = scopeB.GetInstance<IService>("key");

        Assert.NotSame(serviceA1, serviceA2);
        Assert.NotSame(serviceB1, serviceB2);
        Assert.NotSame(serviceA1, serviceB1);
    }


    [Fact]
    public void ShouldHandleKeyedServiceWithEnumServiceKey()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.Register<IKeyedService, KeyedServiceWithEnumServiceKey>(Key.A.ToString(), new PerRootScopeLifetime(rootScope));
        var instance = rootScope.GetInstance<IKeyedService>(Key.A.ToString());
        Assert.Equal(Key.A, ((KeyedServiceWithEnumServiceKey)instance).ServiceKey);
    }

    [Fact]
    public void ShouldHandleKeyedServiceWithIntServiceKey()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.Register<IKeyedService, KeyServiceWithIntServiceKey>("42", new PerRootScopeLifetime(rootScope));
        var instance = rootScope.GetInstance<IKeyedService>("42");
        Assert.Equal(42, ((KeyServiceWithIntServiceKey)instance).ServiceKey);
    }


    // [Fact]
    // public void ResolveKeyedServiceThrowsIfNotSupported()
    // {
    //     var provider = new NonKeyedServiceProvider();
    //     var serviceKey = new object();

    //     Assert.Throws<InvalidOperationException>(() => provider.GetKeyedService<IService>(serviceKey));
    //     Assert.Throws<InvalidOperationException>(() => provider.GetKeyedService(typeof(IService), serviceKey));
    //     Assert.Throws<InvalidOperationException>(() => provider.GetKeyedServices<IService>(serviceKey));
    //     Assert.Throws<InvalidOperationException>(() => provider.GetKeyedServices(typeof(IService), serviceKey));
    //     Assert.Throws<InvalidOperationException>(() => provider.GetRequiredKeyedService<IService>(serviceKey));
    //     Assert.Throws<InvalidOperationException>(() => provider.GetRequiredKeyedService(typeof(IService), serviceKey));
    // }


    [Fact]
    public void Test()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.Register<IFoo, FooWithDependency>("foo");
        container.Register<IBar, Bar>("Bar");
        container.Register<IBar, AnotherBar>("AnotherBar");

        var foo = rootScope.GetInstance<IFoo>("foo");
    }

    // [Fact]
    // public void AnotherTest()
    // {
    //     var serviceCollection = new ServiceCollection();
    //     serviceCollection.AddKeyedSingleton<IService>(KeyedService.AnyKey, (sp, key) => new Service((string)key));

    //     Func<IServiceProvider, object ,IService> factory = (sp, key) => new ServiceWithIntKey((int)key);

    //     factory(null, 32);
    // }


    public interface IKeyedService
    {
    }

    public enum Key
    {
        A,
        B
    }

    public class KeyedServiceWithEnumServiceKey : IKeyedService
    {
        public KeyedServiceWithEnumServiceKey([ServiceKey] Key serviceKey)
        {
            ServiceKey = serviceKey;
        }

        public Key ServiceKey { get; }
    }

    public class KeyServiceWithIntServiceKey : IKeyedService
    {
        public KeyServiceWithIntServiceKey([ServiceKey] int serviceKey)
        {
            ServiceKey = serviceKey;
        }

        public int ServiceKey { get; }
    }


    internal class ServiceFactoryAccessor
    {
        public ServiceFactoryAccessor(IServiceFactory serviceFactory)
        {
            ServiceFactory = serviceFactory;
        }

        public IServiceFactory ServiceFactory { get; }
    }


    internal class OtherServiceWithDefaultCtorArgs
    {
        public OtherServiceWithDefaultCtorArgs(
            [FromKeyedServices("service1")] IService service1 = null,
            [FromKeyedServices("service2")] IService service2 = null)
        {
            Service1 = service1;
            Service2 = service2;
        }

        public IService Service1 { get; }

        public IService Service2 { get; }
    }

    internal class ServiceWithIntKey : IService
    {
        private readonly int _id;

        public ServiceWithIntKey([ServiceKey] int id) => _id = id;
    }


    internal interface IService { }

    internal class Service : IService
    {
        private readonly string _id;

        public Service() => _id = Guid.NewGuid().ToString();

        public Service([ServiceKey] string id) => _id = id;

        public override string? ToString() => _id;
    }

    internal class OtherService
    {
        public OtherService(
            [FromKeyedServices("service1")] IService service1,
            [FromKeyedServices("service2")] IService service2)
        {
            Service1 = service1;
            Service2 = service2;
        }

        public IService Service1 { get; }

        public IService Service2 { get; }
    }


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


/// <summary>
/// A <see cref="ConstructorDependencySelector"/> that looks for the <see cref="InjectAttribute"/> 
/// to determine the name of service to be injected.
/// </summary>
public class AnnotatedConstructorDependencySelector : ConstructorDependencySelector
{
    /// <summary>
    /// Selects the constructor dependencies for the given <paramref name="constructor"/>.
    /// </summary>
    /// <param name="constructor">The <see cref="ConstructionInfo"/> for which to select the constructor dependencies.</param>
    /// <returns>A list of <see cref="ConstructorDependency"/> instances that represents the constructor
    /// dependencies for the given <paramref name="constructor"/>.</returns>
    public override IEnumerable<ConstructorDependency> Execute(ConstructorInfo constructor)
    {
        var constructorDependencies = base.Execute(constructor).ToArray();
        foreach (var constructorDependency in constructorDependencies)
        {
            var injectAttribute =
                (FromKeyedServicesAttribute)
                constructorDependency.Parameter.GetCustomAttributes(typeof(FromKeyedServicesAttribute), true).FirstOrDefault();
            if (injectAttribute != null)
            {
                constructorDependency.ServiceName = injectAttribute.Key.ToString();
            }
        }

        return constructorDependencies;
    }
}

/// <summary>
/// A <see cref="IConstructorSelector"/> implementation that uses information 
/// from the <see cref="InjectAttribute"/> to determine if a given service can be resolved.
/// </summary>
public class AnnotatedConstructorSelector : MostResolvableConstructorSelector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnnotatedConstructorSelector"/> class.
    /// </summary>
    /// <param name="canGetInstance">A function delegate that determines if a service type can be resolved.</param>
    public AnnotatedConstructorSelector(Func<Type, string, bool> canGetInstance)
        : base(canGetInstance)
    {
    }

    /// <summary>
    /// Gets the service name based on the given <paramref name="parameter"/>.
    /// </summary>
    /// <param name="parameter">The <see cref="ParameterInfo"/> for which to get the service name.</param>
    /// <returns>The name of the service for the given <paramref name="parameter"/>.</returns>
    protected override string GetServiceName(ParameterInfo parameter)
    {
        var injectAttribute =
                  (FromKeyedServicesAttribute)
                  parameter.GetCustomAttributes(typeof(FromKeyedServicesAttribute), true).FirstOrDefault();

        return injectAttribute != null ? injectAttribute.Key.ToString() : base.GetServiceName(parameter);
    }
}

