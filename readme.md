# Introduction

LightInject is an ultra lightweight IoC container that supports the most common features expected from a service container.

## The Rationale Behind ##

LightInject was created to provide a very simple, super fast and easy-to-learn service container that can be used in small projects as well as part of larger applications.

LightInject is specifically designed not to bleed into the application code and thus creating a dependency to the container everywhere in the application.

The service container and related types are all marked with the `internal` access modifier and making use of the container outside the containing assembly will actually force the developer to change the modifier from `internal` to `public`

LightInject is also very well suited for stand alone class libraries that requires a service container without creating a dependency to a third-party assembly. We can keep the container within our class library and still ship the library as a single DLL.

LightInject uses Reflection.Emit to dynamically create the code needed to resolve services and dependencies and thus providing performance numbers very close to the new operator.


### Installing - NuGet ###

    PM> Install-Package LightInject

This will install a single code file (`ServiceContainer.cs`) into the current project.

### Terminology ###

* **Service Type** :
	*A type that represents an abstraction of the implementing type.*
* **Implementing Type** 
	*A type that implements the service type.*
* **Default Service** 
	*A service type registered without a service name*
* **Composition root** 
	*A location in the application where services are registered.*   

## About the code ##

The first thing to notice about the code is that every type that is expected to be `public`, is marked with the `internal` access modifier.

This is done intentionally to prevent the types used by the service container to leak out into the application code and hence creating a dependency to this specific service container.

The structure of the code is very functional and great effort has been put into making the codebase as readable and understandable as possible. 

### Thread safety ###

LightInject has been built from the ground up to be 100% thread safe.


## Dependency Injection

Lightinject supports the following types of dependency injection.

* Constructor Injection 
* Property Injection 


## Service Registration ##

There are basically two ways of registering services with the service container.

### Implicit Registration ###

    container.Register(typeof(IFoo),typeof(Foo));

The service type(`IFoo`) is associated with the implementing type(`Foo`) without providing any details about how to create the instance or how to resolve potential dependencies of the `Foo` class.

The container will look for the constructor with the largest number of parameters in the case of a class having multiple constructors.
Any property that is a read-write property will be considered a dependency of the implementing type.

**Note:** The container will throw an `InvalidOperationException` if it encounters any dependency that can not be resolved.

### Explicit Registration ###

```
container.Register<IBar>(c => new Bar());
container.Register<IFoo>(c => new FooWithDependency(container.GetInstance<IBar>()));
```

This allows us to be very specific about hot to resolve the service request as we have provided information about the constructor to use and also how to resolve the dependency.

**Note:** The container will **ONLY** resolve the stated dependencies.

For property injection, we use object initializers:

```
container.Register(typeof(IBar), typeof(Bar));
container.Register<IFoo>(f => new FooWithProperyDependency { Bar = f.GetInstance<IBar>() });
```

### Combining explicit and implicit registration ###

```
container.Register<IBar,Bar>();
container.Register<IFoo>(c => new FooWithDependency(container.GetInstance<IBar>()));
```

This makes it possible to do implicit registration for the simple cases and optionally be more spesific where we want full control of the resolved dependencies.

 
### Assembly Registration ###

Register all services found within the target assembly applying the **Transient** lifecycle.

```
container.RegisterAssembly(someAssembly);
```

Register all services found within the target assembly providing a default lifecycle.

```
container.RegisterAssembly(someAssembly, LifeCycleType.Singleton);
```

Register all services found within the target assembly providing a type filter.

```
container.RegisterAssembly(someAssembly, t => t.Namespace == "SomeNamespace");
```

### No Registration ###

When the container is used without any registered services, the container will register services from the assembly that contains the requested service type.

```
var container = new ServiceContainer();
container.GetInstance<IFoo>();
```

**Note:** This will only happen the first time the `GetInstance` method is invoked. 

### Multiple services ###

LightInject supports multiple services to be registered under the same service type using named services.

```
container.Register<IFoo, Foo>();
container.Register<IFoo, AnotherFoo>("AnotherFoo"); 	
```

Requesting the default service:

```
container.GetService<IFoo>();
```

Requesting the named service:

```
container.GetService<IFoo>("AnotherFoo");
```

If the container has only one named service registration, we can still resolve the instance by requesting the default service.

```
container.Register<IFoo, AnotherFoo>("AnotherFoo"); 	
container.GetService<IFoo>();
```

### Composition root ###

When working with modular applications, it might be necessary to allow the modules to register services with the service container. This can be done by implementing the `ICompositionRoot` interface.   

    public class SampleCompositionRoot : ICompositionRoot
    {               
        public void Compose(IServiceRegistry serviceRegistry)
        {     
            serviceRegistry.Register(typeof(IFoo),typeof(Foo));
        }
    }

When we register an assembly, the container will first look for implementations of the `ICompositionRoot` interface. If one or more implementations are found, they will be created and invoked.

**Note:** Any other services contained within the target assembly that is not registered in the composition root, will **NOT** be registered.

## Custom Factories ##

LightInject provides the `IFactory` interface for implementing custom factories. 

    /// <summary>
    /// Represents a factory class that is capable of returning an object instance.
    /// </summary>    
    internal interface IFactory
    {
        /// <summary>
        /// Returns an instance of the given type indicated by the <paramref name="serviceRequest"/>. 
        /// </summary>        
        /// <param name="serviceRequest">The <see cref="ServiceRequest"/> instance that contains information about the service request.</param>
        /// <returns>An object instance corresponding to the <paramref name="serviceRequest"/>.</returns>
        object GetInstance(ServiceRequest serviceRequest);

        /// <summary>
        /// Determines if this factory can return an instance of the given <paramref name="serviceType"/> and <paramref name="serviceName"/>.
        /// </summary>
        /// <param name="serviceType">The type of the requested service.</param>
        /// <param name="serviceName">The name of the requested service.</param>
        /// <returns><b>true</b>, if the instance can be created, otherwise <b>false</b>.</returns>
        bool CanGetInstance(Type serviceType, string serviceName);
    }
 
A custom factory can be used to resolve unknown services as well as decorating existing services.

The following example shows how to create an instance for which we don't have a registration. 

    public class FooFactory : IFactory
    {
        public object GetInstance(ServiceRequest serviceRequest)
        {
            return new Foo();
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return typeof(IFoo).IsAssignableFrom(serviceType);
        }
    }

We can now retrieve an instance of `Foo` even if we did not register this service with the container.

```
var instance = container.GetInstance<IFoo>();	
Assert.IsNotNull(instance);
```

We can also use a custom factory to decorate an existing service.

    public class FooDecorator : IFoo
    {
        private readonly IFoo foo;

        public FooDecorator(IFoo foo)
        {
            this.foo = foo;
        }

        public IFoo DecoratedInstance
        {
            get
            {
                return this.foo;
            }
        }
    }

The `ServiceRequest` class has a `Proceed` method that is used to resolve the service as registered with the container.

The factory implementation now looks like this:

    public class FooFactory : IFactory
    {
        public object GetInstance(ServiceRequest serviceRequest)
        {       
            return new FooDecorator((IFoo)serviceRequest.Proceed());
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return typeof(IFoo).IsAssignableFrom(serviceType);
        }
    }	 

Finally we need to register the factory and the related services

    container.Register(typeof(IFactory), typeof(FooFactory));
    container.Register(typeof(IFoo), typeof(Foo));
    var instance = container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance, typeof(FooDecorator));

Alternatively, we could use the same factory to return a dynamic proxy object using a third-party proxy library such as Caste DynamicProxy or LinFu DynamicProxy to provide interceptable services.


**Note:** Custom factories are treated as singleton services and we have this in mind with regards to multi-threading.  


## LifeCycle ##
- - - 
### Transient ###

A new instance is returned for each service request.

```
container.Register(typeof(IFoo),typeof(Foo));
var firstinstance = container.GetInstance<IFoo>();
var secondInstance = container.GetInstance<IService>();
Assert.AreNotSame(firstInstance,secondInstance);
```

### Singleton ###

The same instance is returned for each service request.

```
container.Register(typeof(IFoo),typeof(Foo), LifeCycleType.Singleton);
var firstinstance = container.GetInstance<IFoo>();
var secondInstance = container.GetInstance<IService>();
Assert.AreSame(firstInstance,secondInstance);
```

### Request ###

This life cycle type ensures that the same service instance is injected throughout the dependency graph.
This can be used when we want to share an instance, but do not want to use a singleton. 

As an example we have these services where we want the same instance of the `ISampleService` dependency to be injected into both the `FooWithSampleServiceDependency` and the `BarWithSampleServiceDependency` class.

    public class FooWithSampleServiceDependency : IFoo
    {
        public FooWithSampleServiceDependency(IBar bar, ISampleService sampleService)
        {
            SampleService = sampleService;
            Bar = bar;
        }	
        public ISampleService SampleService { get; private set; }
        public IBar Bar { get; private set; }
    }

    public class BarWithSampleServiceDependency : IBar
    {
        public BarWithSampleServiceDependency(ISampleService sampleService)
        {
            SampleService = sampleService;
        }

        public ISampleService SampleService { get; private set; }
    }


If we was to do this manually we would first have to create an instance of the SampleService class and then pass that instance to both constructors.

    var sampleService = new SampleService();
    var bar = new BarWithSampleServiceDependency(sampleService);
    var foo = new FooWithSampleServiceDependency(bar, sampleService);

With LightInject we can register the SampleService with the Request life cycle.

    container.Register<IBar, BarWithSampleServiceDependency>();
    container.Register<ISampleService, SampleService>(LifeCycleType.Request);
    container.Register<IFoo, FooWithSampleServiceDependency>();
    var instance = (FooWithSampleServiceDependency)container.GetInstance<IFoo>();
    Assert.AreSame(((BarWithSampleServiceDependency)instance.Bar).SampleService, instance.SampleService);

### Custom LifeCycle ###

The following example shows how to implement a custom lifecycle that is scoped by a **Transaction**.
   
    public class TransactionScopedFactoryUsingProceed : IFactory
    {
        private readonly ConcurrentDictionary<Transaction, IFoo> instances 
            = new ConcurrentDictionary<Transaction, IFoo>();

        public object GetInstance(ServiceRequest serviceRequest)
        {
            if (Transaction.Current != null)
            {
                return instances.GetOrAdd(Transaction.Current, 
                    t => CreateTransactionScopedInstance(t, serviceRequest));
            }
            return CreateInstance(serviceRequest);
        }

        public bool CanGetInstance(Type serviceType, string serviceName)
        {
            return serviceType == typeof(IFoo);
        }

        private IFoo CreateTransactionScopedInstance(Transaction transaction, ServiceRequest serviceRequest)
        {
            transaction.TransactionCompleted += OnTransactionCompleted;
            return CreateInstance(serviceRequest);
        }

        private IFoo CreateInstance(ServiceRequest serviceRequest)
        {
            return (IFoo)serviceRequest.Proceed();
        }

        private void OnTransactionCompleted(object sender, TransactionEventArgs e)
        {
            e.Transaction.TransactionCompleted -= OnTransactionCompleted;
            IFoo foo;
            instances.TryRemove(e.Transaction, out foo);
        }
    }

As long as we are inside the `TransactionScope`, we will always get the same instance.

        var container = CreateContainer();
        container.Register(typeof(IFactory), typeof(TransactionScopedFactoryUsingProceed));
        container.Register(typeof(IFoo), typeof(Foo));
        using (new TransactionScope())
        {
            var firstInstance = container.GetInstance<IFoo>();
            var secondInstance = container.GetInstance<IFoo>();
            Assert.AreSame(firstInstance, secondInstance);
        }  


**Note:** This technique can also be used to implement other lifecycles such as per `HttpRequest`.


## Open Generic Types ##

Open generic types are generic types where the type parameters has not been specified. The container will create the closed generic type based on the service request.

    container.Register(typeof(IFoo<>), typeof(Foo<>));
    var instance = container.GetInstance(typeof(IFoo<int>));
    Assert.IsInstanceOfType(instance, typeof(Foo<int>));

Generic type parameters can also be used to represent dependencies.

    public class FooWithGenericDependency<T> : IFoo<T>
    {        
        public FooWithGenericDependency(T dependency)
        {
            Dependency = dependency;
        }

        public T Dependency { get; private set; }        
    }

When a service request is made, the container creates the closed generic type and resolves the dependencies.

```
container.Register<IBar, Bar>();
container.Register(typeof(IFoo<>), typeof(FooWithGenericDependency<>));
var instance = (FooWithGenericDependency<IBar>)container.GetInstance<IFoo<IBar>>();
Assert.IsInstanceOfType(instance.Dependency, typeof(Bar));
```

The container will only create a closed generic type if not already present in the container.

This means that we can do a combination of registrering closed generic types and having the container create the type if not found in the container.

    public class FooWithStringTypeParameter : IFoo<string> {}

We can now register both the open and closed generic types.

    container.Register(typeof(IFoo<>), typeof(Foo<>));
    container.Register(typeof(IFoo<string>), typeof(FooWithStringTypeParameter));
    var instance = container.GetInstance(typeof(IFoo<string>));
    Assert.IsInstanceOfType(instance, typeof(FooWithStringTypeParameter));

## Func&lt;T&gt; ##


We can resolve named services from within a service instance by passing a `Func<T>` delegate as the dependency. 

    public class FooWithFuncDependency : IFoo
    {
        public FooWithFuncDependency(Func<IBar> getBar)
        {
            GetBar = getBar;
        }
        public Func<IBar> GetBar { get; private set; } 
    }

The container creates a delegate that is capable of resolving the underlying dependency (`IBar`).

```
container.Register(typeof(IBar), typeof(Bar));
container.Register(typeof(IFoo), typeof(FooWithFuncDependency));
var instance = (FooWithFuncDependency)container.GetInstance<IFoo>();
Assert.IsInstanceOfType(instance.GetBar(), typeof(Bar));
```

## Func&lt;string,T&gt; ##

We can resolve named services from within a service instance by passing a `Func<string, T>` delegate as the dependency. 

    public class FooWithNamedFuncDependency : IFoo
    {
        public FooWithNamedFuncDependency(Func<string, IBar> getBar)
        {
            GetBar = getBar;
        }
        public Func<string,IBar> GetBar { get; private set; }
    }

The container now creates a delegate that is capable of resolving the underlying named dependency.

    var container = CreateContainer();
    container.Register(typeof(IBar), typeof(Bar), "SomeBar");
    container.Register(typeof(IFoo), typeof(FooWithNamedFuncDependency));
    var instance = (FooWithNamedFuncDependency)container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance.GetBar("SomeBar"), typeof(Bar));


## IEnumerable&lt;T&gt; ##

If we have multiple services registered under the same service type, we can have all instances resolved as an `IEnumerable<T>`:

```
container.Register(typeof(IFoo), typeof(Foo));
container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
var services = container.GetInstance<IEnumerable<IFoo>>();
Assert.AreEqual(2, services.Count());
```

`IEnumerable<T>` is also supported as a dependency which means that we can inject a list of services.

    public class FooWithEnumerableDependency : IFoo
    {
        public FooWithEnumerableDependency(IEnumerable<IBar> bars)
        {
            Bars = bars;
        }
        public IEnumerable<IBar> Bars { get; private set; }
    }

We can now register multiple implementations of the `IBar` interface and have them injected as an `IEnumerable<IBar>`	

    container.Register(typeof(IBar), typeof(Bar));
    container.Register(typeof(IBar), typeof(AnotherBar), "AnotherBar");
    container.Register(typeof(IFoo), typeof(FooWithEnumerableDependency));
    var instance = (FooWithEnumerableDependency)container.GetInstance<IFoo>();
    Assert.AreEqual(2, instance.Bars.Count());
	
## Composite pattern ##

The [composite pattern](http://en.wikipedia.org/wiki/Composite_pattern) is a simple pattern that lets a class implement an interface and then delegates invocation of methods to a set other classes implementing the same interface. 

LightInject makes this very simple by letting us inject an `IEnumerable<T>` of the same interface.

    public class FooWithEnumerableIFooDependency : IFoo
    {
        public IEnumerable<IFoo> FooList { get; private set; }

        public FooWithEnumerableIFooDependency(IEnumerable<IFoo> fooList)
        {
            FooList = fooList;
        }
    }

While this looks like a recursive dependency, **LightInject** detects this and removes the  `FooWithEnumerableIFooDependency` from the IEnumerable&lt;IFoo&gt; beeing injected.	 

    container.Register(typeof(IFoo), typeof(Foo), "Foo");
    container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
    container.Register(typeof(IFoo), typeof(FooWithEnumerableIFooDependency));            
    var instance = (FooWithEnumerableIFooDependency)container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance.FooList.First(), typeof(Foo));
    Assert.IsInstanceOfType(instance.FooList.Last(), typeof(AnotherFoo));


## Exceptions ##

**LightInject** does not have any native exception classes and will throw an `InvalidOperationException` if an error occurs.  

LightInject will never fail silently and will throw an `InvalidOperationException` if a service or its dependencies cannot be resolved. 

## Recursive dependency detection ##

A recursive dependency graph is when a service depends directly or indirectly on itself.

    public class FooWithRecursiveDependency : IFoo
    {
        public FooWithRecursiveDependency(IFoo foo)
        {
        }
    }

The folling code will throw an `InvalidOperationException` stating that there are existing recursive dependencies. 

```
container.Register(typeof(IFoo), typeof(FooWithRecursiveDependency));
container.GetInstance<IFoo>()
```

## Mocking ##

It is sometimes useful to be able to mock certain services when doing unit testing.
While services should follow the [Single Responsibility Principle](http://en.wikipedia.org/wiki/Single_responsibility_principle) and be testable as a separate unit, we often need to test top-level classes and in that process we may need to mock services deep down in the dependency graph. 

We could do the setup manually by creating the mock object and make sure that this object gets passed as a dependency where it is needed. This would also require us to manually create the whole dependency graph which could get cumbersome.

When registering services with the container, the container will overwrite any existing registration if a second registration is made with same semantics such as service type and optionally a service name.

This means that we can perform the registration as we normally would do in production and then just mock the services we need.

    container.Register<IFoo, Foo>();
    container.Register<IFoo>(new FooMock());
    var instance = container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance, typeof(FooMock));
