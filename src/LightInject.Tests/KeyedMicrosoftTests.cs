using System;
using System.Collections.Generic;
using System.Linq;
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
        });
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


    internal interface IService { }

    internal class Service : IService
    {
        private readonly string _id;

        public Service() => _id = Guid.NewGuid().ToString();

        public Service([ServiceKey] string id) => _id = id;

        public override string? ToString() => _id;
    }
}