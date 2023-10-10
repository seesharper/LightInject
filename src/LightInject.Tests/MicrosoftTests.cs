using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using LightInject.SampleLibrary;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using Xunit;

namespace LightInject.Tests;

public class MicrosoftTests : TestBase
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
    public void ServicesRegisteredWithImplementationTypeCanBeResolved()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterTransient(typeof(IFoo), typeof(Foo));

        // Act
        var service = container.GetInstance<IFoo>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<Foo>(service);
    }


    [Fact]
    public void ServicesRegisteredWithImplementationType_ReturnDifferentInstancesPerResolution_ForTransientServices()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterTransient(typeof(IFakeService), typeof(FakeService));

        // Act
        var service1 = container.GetInstance<IFakeService>();
        var service2 = container.GetInstance<IFakeService>();

        // Assert
        Assert.IsType<FakeService>(service1);
        Assert.IsType<FakeService>(service2);
        Assert.NotSame(service1, service2);
    }

    [Fact]
    public void ServicesRegisteredWithImplementationType_ReturnSameInstancesPerResolution_ForSingletons()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterSingleton(typeof(IFakeService), typeof(FakeService));

        // Act
        var service1 = container.GetInstance<IFakeService>();
        var service2 = container.GetInstance<IFakeService>();


        // Assert
        Assert.IsType<FakeService>(service1);
        Assert.IsType<FakeService>(service2);
        Assert.Same(service1, service2);
    }

    [Fact]
    public void ServiceInstanceCanBeResolved()
    {
        // Arrange
        var instance = new FakeService();
        var container = CreateContainer();
        container.RegisterInstance<IFakeServiceInstance>(instance);

        // Act
        var service = container.GetInstance<IFakeServiceInstance>();

        // Assert
        Assert.Same(instance, service);
    }

    [Fact]
    public void TransientServiceCanBeResolvedFromProvider()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterTransient(typeof(IFakeService), typeof(FakeService));

        // Act
        var service1 = container.GetInstance<IFakeService>();
        var service2 = container.GetInstance<IFakeService>();

        // Assert
        Assert.NotNull(service1);
        Assert.NotSame(service1, service2);
    }


    [Fact]
    public void TransientServiceCanBeResolvedFromScope()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterTransient(typeof(IFakeService), typeof(FakeService));

        // Act
        var service1 = container.GetInstance<IFakeService>();

        using (var scope = container.BeginScope())
        {
            var scopedService1 = scope.GetInstance<IFakeService>();
            var scopedService2 = scope.GetInstance<IFakeService>();

            // Assert
            Assert.NotSame(service1, scopedService1);
            Assert.NotSame(service1, scopedService2);
            Assert.NotSame(scopedService1, scopedService2);
        }
    }

    [Theory]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Transient)]
    public void NonSingletonService_WithInjectedProvider_ResolvesScopeProvider(ServiceLifetime lifetime)
    {
        // Arrange
        var container = CreateContainer();

        container.RegisterScoped<IFakeService, FakeService>();
        container.RegisterScoped<IServiceFactory>(f => f);
        if (lifetime == ServiceLifetime.Scoped)
        {
            container.RegisterScoped<ClassWithServiceFactory>();
        }
        else
        {
            container.RegisterTransient<ClassWithServiceFactory>();
        }

        // Act
        IFakeService fakeServiceFromScope1 = null;
        IFakeService otherFakeServiceFromScope1 = null;
        IFakeService fakeServiceFromScope2 = null;
        IFakeService otherFakeServiceFromScope2 = null;

        using (var scope1 = container.BeginScope())
        {
            var serviceWithServiceFactory = scope1.GetInstance<ClassWithServiceFactory>();
            fakeServiceFromScope1 = serviceWithServiceFactory.ServiceFactory.GetInstance<IFakeService>();

            serviceWithServiceFactory = scope1.GetInstance<ClassWithServiceFactory>();
            otherFakeServiceFromScope1 = serviceWithServiceFactory.ServiceFactory.GetInstance<IFakeService>();
        }

        using (var scope2 = container.BeginScope())
        {
            var serviceWithServiceFactory = scope2.GetInstance<ClassWithServiceFactory>();
            fakeServiceFromScope2 = serviceWithServiceFactory.ServiceFactory.GetInstance<IFakeService>();

            serviceWithServiceFactory = scope2.GetInstance<ClassWithServiceFactory>();
            otherFakeServiceFromScope2 = serviceWithServiceFactory.ServiceFactory.GetInstance<IFakeService>();
        }

        // Assert
        Assert.Same(fakeServiceFromScope1, otherFakeServiceFromScope1);
        Assert.Same(fakeServiceFromScope2, otherFakeServiceFromScope2);
        Assert.NotSame(fakeServiceFromScope1, fakeServiceFromScope2);
    }

    [Fact]
    public void SingletonServiceCanBeResolvedFromScope()
    {
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.Register<ClassWithServiceFactory>(new RootScopeLifetime(rootScope));
        container.RegisterScoped<IServiceFactory>(f => f);

        // Act
        IServiceFactory scopedSp1 = null;
        IServiceFactory scopedSp2 = null;
        ClassWithServiceFactory instance1 = null;
        ClassWithServiceFactory instance2 = null;

        using (var scope1 = container.BeginScope())
        {
            scopedSp1 = scope1.GetInstance<IServiceFactory>();
            instance1 = scope1.GetInstance<ClassWithServiceFactory>();
        }

        using (var scope2 = container.BeginScope())
        {
            scopedSp2 = scope2.GetInstance<IServiceFactory>();
            instance2 = scope2.GetInstance<ClassWithServiceFactory>();
        }

        // Assert
        Assert.Same(instance1, instance2);
        Assert.Same(instance1.ServiceFactory, instance2.ServiceFactory);
        Assert.NotSame(instance1.ServiceFactory, scopedSp1);
        Assert.NotSame(instance2.ServiceFactory, scopedSp2);
    }

    [Fact]
    public void SingleServiceCanBeIEnumerableResolved()
    {
        // Arrange
        var container = new ServiceContainer();
        container.RegisterTransient<IFakeService, FakeService>();

        // Act
        var services = container.GetInstance<IEnumerable<IFakeService>>();

        // Assert
        Assert.NotNull(services);
        var service = Assert.Single(services);
        Assert.IsType<FakeService>(service);
    }

    [Fact]
    public void MultipleServiceCanBeIEnumerableResolved()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterTransient<IFakeMultipleService, FakeOneMultipleService>();
        container.RegisterTransient<IFakeMultipleService, FakeTwoMultipleService>();

        // Act
        var services = container.GetInstance<IEnumerable<IFakeMultipleService>>();

        // Assert
        Assert.Collection(services.OrderBy(s => s.GetType().FullName),
            service => Assert.IsType<FakeOneMultipleService>(service),
            service => Assert.IsType<FakeTwoMultipleService>(service));
    }

    [Fact]
    public void RegistrationOrderIsPreservedWhenServicesAreIEnumerableResolved()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterTransient<IFakeMultipleService, FakeOneMultipleService>();
        container.RegisterTransient<IFakeMultipleService, FakeTwoMultipleService>();


        var containerReversed = CreateContainer();
        containerReversed.RegisterTransient<IFakeMultipleService, FakeTwoMultipleService>();
        containerReversed.RegisterTransient<IFakeMultipleService, FakeOneMultipleService>();

        // Act
        var services = container.GetInstance<IEnumerable<IFakeMultipleService>>();
        var servicesReversed = containerReversed.GetInstance<IEnumerable<IFakeMultipleService>>();

        // Assert
        Assert.Collection(services,
            service => Assert.IsType<FakeOneMultipleService>(service),
            service => Assert.IsType<FakeTwoMultipleService>(service));

        Assert.Collection(servicesReversed,
            service => Assert.IsType<FakeTwoMultipleService>(service),
            service => Assert.IsType<FakeOneMultipleService>(service));
    }

    [Fact]
    public void OuterServiceCanHaveOtherServicesInjected()
    {
        // Arrange
        var fakeService = new FakeService();
        var container = CreateContainer();
        container.RegisterTransient<IFakeOuterService, FakeOuterService>();
        container.RegisterInstance<IFakeService>(fakeService);
        container.RegisterTransient<IFakeMultipleService, FakeOneMultipleService>();
        container.RegisterTransient<IFakeMultipleService, FakeTwoMultipleService>();

        // Act
        var services = container.GetInstance<IFakeOuterService>();

        // Assert
        Assert.Same(fakeService, services.SingleService);
        Assert.Collection(services.MultipleServices.OrderBy(s => s.GetType().FullName),
            service => Assert.IsType<FakeOneMultipleService>(service),
            service => Assert.IsType<FakeTwoMultipleService>(service));
    }

    [Fact]
    public void FactoryServicesCanBeCreatedByGetService()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterTransient<IFakeService, FakeService>();

        container.RegisterTransient<IFactoryService>(sf =>
        {
            var fakeService = sf.GetInstance<IFakeService>();
            return new TransientFactoryService
            {
                FakeService = fakeService,
                Value = 42
            };
        });

        // Act
        var service = container.GetInstance<IFactoryService>();

        // Assert
        Assert.NotNull(service);
        Assert.Equal(42, service.Value);
        Assert.NotNull(service.FakeService);
        Assert.IsType<FakeService>(service.FakeService);
    }

    [Fact]
    public void FactoryServicesAreCreatedAsPartOfCreatingObjectGraph()
    {
        // Arrange
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.RegisterTransient<IFakeService, FakeService>();
        container.RegisterTransient<IFactoryService>(sf =>
        {
            var fakeService = sf.GetInstance<IFakeService>();
            return new TransientFactoryService
            {
                FakeService = fakeService,
                Value = 42
            };
        });
        container.RegisterScoped(sf =>
        {
            var fakeService = sf.GetInstance<IFakeService>();
            return new ScopedFactoryService
            {
                FakeService = fakeService,
            };
        });

        container.RegisterTransient<ServiceAcceptingFactoryService>();

        // Act
        var service1 = container.GetInstance<ServiceAcceptingFactoryService>();
        var service2 = container.GetInstance<ServiceAcceptingFactoryService>();

        // Assert
        Assert.Equal(42, service1.TransientService.Value);
        Assert.NotNull(service1.TransientService.FakeService);

        Assert.Equal(42, service2.TransientService.Value);
        Assert.NotNull(service2.TransientService.FakeService);

        Assert.NotNull(service1.ScopedService.FakeService);

        // Verify scoping works
        Assert.NotSame(service1.TransientService, service2.TransientService);
        Assert.Same(service1.ScopedService, service2.ScopedService);
    }

    [Fact]
    public void LastServiceReplacesPreviousServices()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterTransient<IFakeMultipleService, FakeOneMultipleService>();
        container.RegisterTransient<IFakeMultipleService, FakeTwoMultipleService>();

        // Act
        var service = container.GetInstance<IFakeMultipleService>();

        // Assert
        Assert.IsType<FakeTwoMultipleService>(service);
    }

    [Fact]
    public void SingletonServiceCanBeResolved()
    {
        // Arrange
        var container = CreateContainer();
        var rootScope = container.BeginScope();

        container.Register<IFakeSingletonService, FakeService>(new RootScopeLifetime(rootScope));

        // Act
        var service1 = container.GetInstance<IFakeSingletonService>();
        var service2 = container.GetInstance<IFakeSingletonService>();

        // Assert
        Assert.NotNull(service1);
        Assert.Same(service1, service2);
    }

    // Note: These tests are not relevant here. Consider moving them to LightInject.Microsoft.DependencyInjection.Tests
    // public void ServiceProviderRegistersServiceScopeFactory()
    // public void ServiceScopeFactoryIsSingleton()

    [Fact]
    public void ScopedServiceCanBeResolved()
    {
        // Arrange
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.RegisterScoped<IFakeScopedService, FakeService>();

        // Act
        using (var scope = container.BeginScope())
        {
            var providerScopedService = rootScope.GetInstance<IFakeScopedService>();
            var scopedService1 = scope.GetInstance<IFakeScopedService>();
            var scopedService2 = scope.GetInstance<IFakeScopedService>();

            // Assert
            Assert.NotSame(providerScopedService, scopedService1);
            Assert.Same(scopedService1, scopedService2);
        }
    }

    [Fact]
    public void NestedScopedServiceCanBeResolved()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterScoped<IFakeScopedService, FakeService>();

        // Act
        using (var outerScope = container.BeginScope())
        using (var innerScope = outerScope.BeginScope())
        {
            var outerScopedService = outerScope.GetInstance<IFakeScopedService>();
            var innerScopedService = innerScope.GetInstance<IFakeScopedService>();

            // Assert
            Assert.NotNull(outerScopedService);
            Assert.NotNull(innerScopedService);
            Assert.NotSame(outerScopedService, innerScopedService);
        }
    }

    // Note: These tests are not relevant here. Consider moving them to LightInject.Microsoft.DependencyInjection.Tests
    // public void ScopedServices_FromCachedScopeFactory_CanBeResolvedAndDisposed()

    [Fact]
    public void ScopesAreFlatNotHierarchical()
    {
        // Arrange
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.Register<IFakeSingletonService, FakeService>(new RootScopeLifetime(rootScope));

        // Act
        var outerScope = container.BeginScope();
        using var innerScope = outerScope.BeginScope();
        outerScope.Dispose();
        var innerScopedService = innerScope.GetInstance<IFakeSingletonService>();

        // Assert
        Assert.NotNull(innerScopedService);
    }

    // Note: These tests are not relevant here. Consider moving them to LightInject.Microsoft.DependencyInjection.Tests
    //  public void ServiceProviderIsDisposable()


    [Fact]
    public void DisposingScopeDisposesService()
    {
        // Arrange

        var container = CreateContainer();
        var rootScope = container.BeginScope();
        FakeService disposableService;
        FakeService transient1;
        FakeService transient2;
        FakeService transient3;
        FakeService singleton;

        container.Register<IFakeSingletonService, FakeService>(new RootScopeLifetime(rootScope));
        container.RegisterScoped<IFakeScopedService, FakeService>();
        container.Register<IFakeService, FakeService>(new PerRequestLifeTime());

        // Act and Assert
        transient3 = Assert.IsType<FakeService>(rootScope.GetInstance<IFakeService>());
        using (var scope = container.BeginScope())
        {
            disposableService = (FakeService)scope.GetInstance<IFakeScopedService>();
            transient1 = (FakeService)scope.GetInstance<IFakeService>();
            transient2 = (FakeService)scope.GetInstance<IFakeService>();
            singleton = (FakeService)scope.GetInstance<IFakeSingletonService>();

            Assert.False(disposableService.Disposed);
            Assert.False(transient1.Disposed);
            Assert.False(transient2.Disposed);
            Assert.False(singleton.Disposed);
        }


        Assert.True(disposableService.Disposed);
        Assert.True(transient1.Disposed);
        Assert.True(transient2.Disposed);
        Assert.False(singleton.Disposed);

        (rootScope as IDisposable).Dispose();

        Assert.True(singleton.Disposed);
        Assert.True(transient3.Disposed);
    }

    // Note: These tests are not relevant here. Consider moving them to LightInject.Microsoft.DependencyInjection.Tests
    // public void SelfResolveThenDispose()
    // public void SafelyDisposeNestedProviderReferences()

    [Fact]
    public void SingletonServicesComeFromRootProvider()
    {
        // Arrange
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.Register<IFakeSingletonService, FakeService>(new RootScopeLifetime(rootScope));

        FakeService disposableService1;
        FakeService disposableService2;

        // Act and Assert
        using (var scope = container.BeginScope())
        {
            var service = scope.GetInstance<IFakeSingletonService>();
            disposableService1 = Assert.IsType<FakeService>(service);
            Assert.False(disposableService1.Disposed);
        }

        Assert.False(disposableService1.Disposed);

        using (var scope = container.BeginScope())
        {
            var service = scope.GetInstance<IFakeSingletonService>();
            disposableService2 = Assert.IsType<FakeService>(service);
            Assert.False(disposableService2.Disposed);
        }

        Assert.False(disposableService2.Disposed);
        Assert.Same(disposableService1, disposableService2);
    }

    [Fact]
    public void NestedScopedServiceCanBeResolvedWithNoFallbackProvider()
    {
        // Arrange
        var container = CreateContainer();
        container.RegisterScoped<IFakeScopedService, FakeService>();


        // Act
        using (var outerScope = container.BeginScope())
        using (var innerScope = outerScope.BeginScope())
        {
            var outerScopedService = outerScope.GetInstance<IFakeScopedService>();
            var innerScopedService = innerScope.GetInstance<IFakeScopedService>();

            // Assert
            Assert.NotSame(outerScopedService, innerScopedService);
        }
    }

    [Fact]
    public void OpenGenericServicesCanBeResolved()
    {
        // Arrange
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.Register<IFakeSingletonService, FakeService>(new RootScopeLifetime(rootScope));
        container.RegisterTransient(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>));

        // Act
        var genericService = container.GetInstance<IFakeOpenGenericService<IFakeSingletonService>>();
        var singletonService = container.GetInstance<IFakeSingletonService>();

        // Assert
        Assert.Same(singletonService, genericService.Value);
    }

    [Fact]
    public void ConstrainedOpenGenericServicesCanBeResolved()
    {
        // Arrange
        var container = CreateContainer();
        var rootScope = container.BeginScope();
        container.RegisterTransient(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>));
        container.RegisterTransient(typeof(IFakeOpenGenericService<>), typeof(ConstrainedFakeOpenGenericService<>));
        
        var poco = new PocoClass();
        container.RegisterInstance(poco);
        container.Register<IFakeSingletonService, FakeService>(new RootScopeLifetime(rootScope));
        
        // Act
        var allServices = container.GetAllInstances<IFakeOpenGenericService<PocoClass>>().ToList();
        var constrainedServices = container.GetAllInstances<IFakeOpenGenericService<IFakeSingletonService>>().ToList();
        var singletonService = container.GetInstance<IFakeSingletonService>();
        // Assert
        Assert.Equal(2, allServices.Count);
        Assert.Same(poco, allServices[0].Value);
        Assert.Same(poco, allServices[1].Value);
        Assert.Equal(1, constrainedServices.Count);
        Assert.Same(singletonService, constrainedServices[0].Value);
    }



    [Fact]
    public void ShouldAllowMultipleRegistrations()
    {
        var container = CreateContainer();
        container.Register<IFoo, AnotherFoo>();
        container.Register<IFoo, Foo>();

        var instances = container.GetAllInstances<IFoo>();
        Assert.Equal(2, instances.Count());

        var foo = container.GetInstance<IFoo>();
        Assert.IsType<Foo>(foo);
    }

    [Fact]
    public void ShouldAllowMultipleNamedRegistrations()
    {
        var container = CreateContainer();

    }
}

public class ClassWithServiceFactory
{
    public ClassWithServiceFactory(IServiceFactory serviceFactory)
    {
        ServiceFactory = serviceFactory;
    }

    public IServiceFactory ServiceFactory { get; }
}

/// <summary>
/// An <see cref="ILifetime"/> implementation that makes it possible to mimic the notion of a root scope.
/// </summary>
[LifeSpan(30)]
internal class RootScopeLifetime : ILifetime, ICloneableLifeTime
{
    private readonly object syncRoot = new object();
    private readonly Scope rootScope;
    private object instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerRootScopeLifetime"/> class.
    /// </summary>
    /// <param name="rootScope">The root <see cref="Scope"/>.</param>
    public RootScopeLifetime(Scope rootScope)
        => this.rootScope = rootScope;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public object GetInstance(Func<object> createInstance, Scope scope)
        => throw new NotImplementedException("Uses optimized non closing method");

    /// <inheritdoc/>
    public ILifetime Clone()
        => new PerRootScopeLifetime(rootScope);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE0060
    public object GetInstance(GetInstanceDelegate createInstance, Scope scope, object[] arguments)
    {
#pragma warning restore IDE0060
        if (instance != null)
        {
            return instance;
        }

        lock (syncRoot)
        {
            if (instance == null)
            {
                instance = createInstance(arguments, rootScope);
                RegisterForDisposal(instance);
            }
        }

        return instance;
    }

    private void RegisterForDisposal(object instance)
    {
        if (instance is IDisposable disposable)
        {
            rootScope.TrackInstance(disposable);
        }
        else if (instance is IAsyncDisposable asyncDisposable)
        {
            rootScope.TrackInstance(asyncDisposable);
        }
    }
}
