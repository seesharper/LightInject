[![AppVeyor](https://img.shields.io/appveyor/ci/seesharper/lightinject.svg?maxAge=2592000)](https://ci.appveyor.com/project/seesharper/lightinject/branch/master)
[![NuGet](https://img.shields.io/nuget/v/LightInject.svg?maxAge=2592000)](https://www.nuget.org/packages/LightInject)
[![GitHub tag](https://img.shields.io/github/tag/seesharper/lightinject.svg?maxAge=2592000)](https://github.com/seesharper/LightInject/releases/latest)

<a href="https://www.buymeacoffee.com/Y3bqWk1" target="_blank"><img src="https://bmc-cdn.nyc3.digitaloceanspaces.com/BMC-button-images/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>

## Installing ##

**LightInject** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
        <code>PM&gt; Install-Package LightInject</code>
   </p>
</div>

This adds a reference to the LightInject.dll in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
        <code>PM&gt; Install-Package LightInject.Source </code>
   </p>
</div>

This will install a single file (LightInject.cs) into the current project.

### Creating a container ###

```c#
var container = new LightInject.ServiceContainer();
```

The container implements IDisposable and should be disposed after usage has completed. It can also be used inside of a using statement for a constrained scope.

### Default services ###

```c#
public interface IFoo {}
public class Foo : IFoo {}
```

---

```c#
container.Register<IFoo, Foo>();
var instance = container.GetInstance<IFoo>();
Assert.IsInstanceOfType(instance, typeof(Foo));
```

### Named services ###

```c#
public class Foo : IFoo {}
public class AnotherFoo : IFoo {}
```

---

```c#
container.Register<IFoo, Foo>();
container.Register<IFoo, AnotherFoo>("AnotherFoo");
var instance = container.GetInstance<IFoo>("AnotherFoo");
Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
```

If only one named registration exists, **LightInject** is capable of resolving this as the default service.

```c#
container.Register<IFoo, AnotherFoo>("AnotherFoo");
var instance = container.GetInstance<IFoo>();
Assert.IsInstanceOfType(instance, typeof(AnotherFoo));
```

### Unresolved services ###

LightInject can resolve services that are not registered with the container using the *RegisterFallback* method.

```c#
var container = new ServiceContainer();
container.RegisterFallback((type, s) => true, request => new Foo());
var foo = container.GetInstance<IFoo>();
```

The first argument to the *RegisterFallback* method makes it possible to decide if the service can be "late-resolved".
The second argument is a *ServiceRequest* instance that provides the requested service type and service name.

### IEnumerable&lt;T&gt; ###

When we register multiple services with the same service type, **LightInject** is capable of resolving these services as an  [IEnumerable&lt;T&gt;](http://msdn.microsoft.com/en-us/library/9eekhta0.aspx).

```c#
public class Foo : IFoo {}
public class AnotherFoo : IFoo {}
```

---

```c#
container.Register<IFoo, Foo>();
container.Register<IFoo, AnotherFoo>("AnotherFoo");
var instances = container.GetInstance<IEnumerable<IFoo>>();
Assert.AreEqual(2, instances.Count());
```

Alternatively using the **GetAllInstances** method.

```c#
var instances = container.GetAllInstances<IFoo>();
Assert.AreEqual(2, instances.Count());
```

In addition, **LightInject** supports the following [IEnumerable&lt;T&gt;](http://msdn.microsoft.com/en-us/library/9eekhta0.aspx) sub-types. 

* Array
* ICollection&lt;T&gt;
* IList&lt;T&gt;
* IReadOnlyCollection&lt;T&gt; (Net 4.5 and Windows Runtime);
* IReadOnlyList&lt;T&gt; (Net 4.5 and Windows Runtime)

By default, **LightInject** will resolve all services that are compatible with the requested element type.

```c#
container.Register<Foo>();
container.Register<DerivedFoo>();
var instances = container.GetAllInstances<Foo>();
Assert.AreEqual(2, instances.Count());
```

This behavior can be overridden using the **EnableVariance** container option.

```c#
var container = new ServiceContainer(new ContainerOptions { EnableVariance = false });
container.Register<Foo>();
container.Register<DerivedFoo>();
var instances = container.GetAllInstances<Foo>();
Assert.AreEqual(1, instances.Count());
```

We can also selectively decide to apply variance only for certain `IEnumerable<T>` services.

```C#
options.VarianceFilter = (enumerableType) => enumerableType.GetGenericArguments()[0] == typeof(IFoo);
```



#### Ordering

Sometimes the ordering of the resolved services are important and **LightInject** solves this by ordering services by their service name.

```c#
container.Register<IFoo, Foo1>("A");
container.Register<IFoo, Foo2>("B");
container.Register<IFoo, Foo3>("C");

var instances = container.GetAllInstances<IFoo>().ToArray();
Assert.IsType<Foo1>(instances[0]);
Assert.IsType<Foo2>(instances[1]);
Assert.IsType<Foo3>(instances[2]);
```

We can also register multiple implementations for a given service type using the `RegisterOrdered` method.

```c#
var container = CreateContainer();
container.RegisterOrdered(
    typeof(IFoo),
    new[] {typeof(Foo1), typeof(Foo2), typeof(Foo3)},
    type => new PerContainerLifetime());

var instances = container.GetAllInstances<IFoo>().ToArray();

Assert.IsType<Foo1>(instances[0]);
Assert.IsType<Foo2>(instances[1]);
Assert.IsType<Foo3>(instances[2]);
```

The `RegisterOrdered` method gives each implementation a service name that can be used for ordering when resolving these services. By default the service name is formatted like `001`, `002` and so on. 
If we need so change this convention, we can do this by passing a format function to the `RegisterOrdered` method.

```c#
container.RegisterOrdered(
    typeof(IFoo<>),
    new[] { typeof(Foo1<>), typeof(Foo2<>), typeof(Foo3<>) },
    type => new PerContainerLifetime(), i => $"A{i.ToString().PadLeft(3,'0')}");

var services = container.AvailableServices.Where(sr => sr.ServiceType == typeof(IFoo<>))
    .OrderBy(sr => sr.ServiceName).ToArray();
Assert.Equal("A001", services[0].ServiceName);
Assert.Equal("A002", services[1].ServiceName);
Assert.Equal("A003", services[2].ServiceName);
```

### Values ###

Registers the value as a constant.

```c#
container.RegisterInstance<string>("SomeValue");
var value = container.GetInstance<string>();
Assert.AreEqual("SomeValue, value");
```



### Compilation

**LightInject** uses dynamic code compilation either in the form of System.Reflection.Emit or compiled expression trees. When a service is requested from the container, the code needed for creating the service instance is generated and compiled and a delegate for that code is stored for lookup later on so that we only compile it once. These delegates are stored in an AVL tree that ensures maximal performance when looking up a delegate for a given service type. If fact, looking up these delegates is what sets the top performing containers apart. Most high performance container emits approximately the same code, but the approach to storing these delegates may differ. 

**LightInject** provides lock-free service lookup meaning that no locks are involved for getting a service instance after its initial generation and compilation. The only time **LightInject** actually creates a lock is when generating the code for a given service.  That does however mean a potential lock contention problem when many concurrent requests asks for services for the first time.

**LightInject** deals with this potential problem by providing an API for compilation typically used when an application starts.

The following example shows how to compile all registered services.

```c#
container.Compile();
```

One thing to be aware of is that not all services are backed by its own delegate. 

Consider the following service:

```c#
public class Foo
{
    public Foo(Bar bar)
    {
        Bar = bar;
    }
} 
```

Registered and resolved like this:

```c#
container.Register<Foo>();
container.Register<Bar>();
var foo = container.GetInstance<Foo>();
```

In this case we only create a delegate for resolving `Foo` since that is the only service that is directly requested from the container. The code for creating the `Bar` instance is embedded inside the code for creating the `Foo` instance and hence there is only one delegate created.

We call `Foo` a root service since it is directly requested from the container.

In fact lets just have a look at the IL generated for creating the `Foo` instance. 

```assembly
newobj Void .ctor() // Bar
newobj Void .ctor(LightInject.SampleLibrary.IBar) //Foo
```

What happens here is that a new instance of `Bar` is created and pushed onto the stack and then we create the `Foo` instance. This is the code that the delegate for `Foo` points to. 

The reason for such a relatively detailed explanation is to illustrate that we don't always create a delegate for a given service and by simply doing a `container.Compile()` we might create a lot of delegates that is never actually executed.  Probably no big deal as long as we don't have tens of thousands of services, but just something to be aware of.

**LightInject** does not attempt to identify root services as that would be very difficult for various reasons.

We can instead use a predicate when compiling services up front.

```C#
container.Compile(sr => sr.ServiceType == typeof(Foo));
```

#### Open Generics

**LightInject** cannot compile open generic services since the actual generic arguments are not known at "compile" time. 

We can however specify the generic arguments like this:

```c#
container.Compile<Foo<int>>()
```

**LightInject** will create a log entry every time a new delegate is created so that information can be used to identify root services that could be compiled up front. In addition to this, a log entry (warning) is also created when trying to compile an open generic service up front.



## Lifetime ##

The default behavior in **LightInject** is to treat all objects as transients unless otherwise specified.

```c#
container.Register<IFoo,Foo>();
var firstInstance = container.GetInstance<IFoo>();
var secondInstance = container.GetInstance<IFoo>();
Assert.AreNotSame(firstInstance, secondInstance);
```

### PerScopeLifetime ###

Ensures that only one instance of a given service can exists within a scope.
The container will call the **Dispose** method on all disposable objects created within the scope.

```c#
container.Register<IFoo,Foo>(new PerScopeLifetime());
using(container.BeginScope())
{    
    var firstInstance = container.GetInstance<IFoo>();
    var secondInstance = container.GetInstance<IFoo>();
    Assert.AreSame(firstInstance, secondInstance);
}
```

**Note:** *An **InvalidOperationException** is thrown if a service registered with the **PerScopeLifetime** is requested outside the scope.*

### PerContainerLifetime ###

Ensures that only one instance of a given service can exist within the container.
The container will call the Dispose method on all disposable objects when the container itself is disposed.

```c#
using(container = new ServiceContainer())
{
    container.Register<IFoo,Foo>(new PerContainerLifetime());    
    var firstInstance = container.GetInstance<IFoo>();
    var secondInstance = container.GetInstance<IFoo>();
    Assert.AreSame(firstInstance, secondInstance);
}
```

### PerRequestLifeTime ###

A new instance is created for each request and the container calls **Dispose** when the scope ends.
This lifetime is used when the conrete class implements **IDisposable**.

```c#
container.Register<IFoo,Foo>(new PerRequestLifeTime());
using(container.BeginScope())
{        
    var firstInstance = container.GetInstance<IFoo>();
    var secondInstance = container.GetInstance<IFoo>();
    Assert.AreNotSame(firstInstance, secondInstance);
}    
```

>**Note:** *An **InvalidOperationException** is thrown if a service registered with the **PerRequestLifeTime** is requested outside the scope.*


### Custom lifetime ###

A custom lifetime is created by implementing the **ILifetime** interface

```c#
internal interface ILifetime
{
    object GetInstance(Func<object> instanceFactory, Scope currentScope);        
}
```

The following example shows to create a custom lifetime that ensures only one instance per thread.

```c#
public class PerThreadLifetime : ILifetime
{
    ThreadLocal<object> instances = new ThreadLocal<object>();     

    public object GetInstance(Func<object> instanceFactory, Scope currentScope)
    {
        if (instances.value == null)
        {
            instances.value = instanceFactory();
        }
        return instances.value;
    }
}
```

That is all it takes to create a custom lifetime, but what about disposable services?

```c#
public class PerThreadLifetime : ILifetime
{
    ThreadLocal<object> instances = new ThreadLocal<object>();     

    public object GetInstance(Func<object> instanceFactory, Scope currentScope)
    {            
        if (instances.value == null)
        {                
            object instance = instanceFactory();                
            IDisposable disposable = instance as IDisposable;                
            if (disposable != null)
            {
                if (currentScope == null)
                {
                    throw new InvalidOperationException(
                        "Attempt to create an disposable object without a current scope.");
                }
                currentScope.TrackInstance(disposable);
            }

            instances.value = instance;
        }
        return instance.value;
    }
}
```

#### Important ####

A lifetime object controls the lifetime of a single service and can **never** be shared for multiple service registrations.

**Wrong**

```c#
ILifetime lifetime = new PerContainerLifeTime();
container.Register<IFoo,Foo>(lifetime);
container.Register<IBar,Bar>(lifetime);
```

**Right**

```c#
container.Register<IFoo,Foo>(new PerContainerLifeTime());
container.Register<IBar,Bar>(new PerContainerLifeTime());
```

A lifetime object is also shared across threads and that is something we must take into consideration when developing new lifetime implementations.

### Async and Await ###

By default scopes are managed per thread which means that when the container looks for the current scope, it will look for a scope that is associated with the current thread.

With the introduction of the async/await pattern chances are that the code that is requesting a service instance is running on another thread.

To illustrate this lets consider an example that is going to cause an instance to be resolved on another thread.

We start of by creating an interface that returns a **Task&lt;IBar&gt;**

```c#
public interface IAsyncFoo
{
    Task<IBar> GetBar();
}
```

Next we implement this interface in such a way that the **IBar** instance is requested on another thread.

```c#
public class AsyncFoo : IAsyncFoo
{
    private readonly Lazy<IBar> lazyBar;

    public AsyncFoo(Lazy<IBar> lazyBar)
    {
        this.lazyBar = lazyBar;
    }

    public async Task<IBar> GetBar()
    {
        await Task.Delay(10);
        return lazyBar.Value; <--This code is executed on another thread (continuation).
    }
}
```

The we register the dependency (**IBar**) with the **PerScopeLifetime** that is going to cause the container to ask for the current scope so that the instance can be registered with that scope.

```c#
var container = new ServiceContainer();
container.Register<IBar, Bar>(new PerScopeLifetime());
container.Register<IAsyncFoo, AsyncFoo>();

using (container.BeginScope())
{
    var instance = container.GetInstance<IAsyncFoo>();
    ExceptionAssert.Throws<AggregateException>(() => instance.GetBar().Wait());                
}
```

This will throw an exception that states the following:

    Attempt to create a scoped instance without a current scope.  

The reason that this is happening is that the current scope is associated with the thread that created it and when the continuation executes, we are essentially requesting an instance on another thread.

To deal with this issue, **LightInject** now supports scopes across the logical [CallContext](http://msdn.microsoft.com/en-us/library/system.runtime.remoting.messaging.callcontext(v=vs.110).aspx).  

```c#
var container = new ServiceContainer();
container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
container.Register<IBar, Bar>(new PerScopeLifetime());
container.Register<IAsyncFoo, AsyncFoo>();

using (container.BeginScope())
{
    var instance = container.GetInstance<IAsyncFoo>();
    var bar = instance.GetBar().Result;
    Assert.IsInstanceOfType(bar, typeof(IBar));
}
```

> Note that the **PerLogicalCallContextScopeManagerProvider** is only available when running under .Net 4.5.
> For more information, please refer to the following [article](http://blog.stephencleary.com/2013/04/implicit-async-context-asynclocal.html) by Stephen Cleary.



## Scope 

The purpose of the scope is to track the services created within the scope. For instance, the `PerScopeLifetime` uses the scope to ensure that we only create a single service instance even if it requested multiple times.

One of the most canonical examples would be in a web application where we need to inject `IDbConnection` into different services. Let say that we have an `OrderController` and we need two services to process the order.

```c#
public class CustomerService : ICustomerService
{
    public CustomerService(IDbConnection dbConnection)
    {
    }
}
```

```C#
public class OrderService : IOrderService
{
    public OrderService(IDbConnection dbConnection)
    {
    }
}  
```



```C#
public class OrderController
{
    public OrderController(ICustomerService customerService, IOrderService orderService)
    {
    }
}
```

As we can see the `OrderController` depends on both the `CustomerService` and the `OrderService` which are both dependant upon an `IDbConnection`. 

By registering the `IDbConnection` as a scoped service we ensure two things.

- Only a single instance of `IDbConnection` will ever be created inside a scope. 
- The `IDbConnection` instance is disposed when the scope ends.

```c#
container.RegisterScoped<IDbConnection>(factory => new ProviderSpecificConnection());
```

So when and how do we start these scopes?

She short answer is that most of the time, we don't. For instance, in AspNetCore, the scopes are started and ended by the AspNetCore infrastructure so we don't have to think about that when developing web application. A scope is started when the web request starts and it ended when the web request ends. It is really that simple, one request equals one scope.

So in **LightInject**, we register a scoped service using `RegisterScoped` without really thinking about when and how the scopes are started and ended. In a web application this usually means a web request, but for other applications it can mean something else. Maybe for a UI application it means a page/window/form or something similar. 

To start a scope manually we can create scope using the `BeginScope` method

```c#
using (container.BeginScope())
{
    var dbConnection = container.GetInstance<IDbConnection>();  
}        
```

> Note: The `Scope` implement `IDisposable` and should always be wrapped in a using block to ensure its disposal

In this example we start a new scope and retrieve the service from the container which means that **LightInject** uses the "current" scope to resolve the service. This is only supported for backwards compatibility and should be avoided if possible. The recommended approach is to retrieve services directly from the scope.

```c#
using (var scope = container.BeginScope())
{
    var dbConnection = scope.GetInstance<IDbConnection>();  
}    
```

Since we are retrieving the service directly from the scope, the current scope is ignored and we simply use the scope from which the service was requested. This is not only much faster, but it is also a much safer way to deal with scopes.

This also allows for multiple active scopes.

```C#
using (var outerScope = container.BeginScope())
{    
    using (var innerScope = container.BeginScope())
    {
        var outerDbConnection = outerScope.GetInstance<IDbConnection>();
        var innerDbConnection = innerScope.GetInstance<IDbConnection>();
    }  
}    
```

In addition to the `PerScopeLifetime` which ensures disposal and a single instance within a scope, we also have the `PerRequestLifetime`. This lifetime behaves just a transient meaning that we get a new instance for every time it is requested with the only difference to transients being that instances are disposed when the scope ends.

> Note: The `PerRequestLifetime` has NO relation to the notion of a web request. 

If we don't need access to an ambient scope, we can disable this in the `ContainerOptions`

```csharp
var container = new ServiceContainer(o => o.EnableCurrentScope = false);
```

This also improves performance ever so slightly as we don't need to maintain a current scope when scopes are started and ended. 

## Dependencies ##


### Constructor Injection ##

```c#
public interface IFoo {}        
public interface IBar {}

public class Foo : IFoo
{
    public Foo(IBar bar) 
    {
        Bar = bar;
    }

    public IBar Bar { get; private set; } 
}

public class Bar : IBar {}
```

#### Implicit service registration ####

Registers a service without specifying any information about how to resolve the constructor dependencies of the implementing type.

```c#
container.Register<IFoo, Foo>();
container.Register<IBar, Bar>();
var foo = (Foo)container.GetInstance<IFoo>();
Assert.IsInstanceOfType(foo.Bar, typeof(Bar)); 
```

> Note: In the case where the implementing type(Foo) has more than one constructor, **LightInject** will choose the constructor with the most parameters. 

For fine grained control of the injected constructor dependencies, we can provide a factory that makes it possible to create an instance of a given constructor dependency.

```c#
container.RegisterConstructorDependency<IBar>((factory, parameterInfo) => new Bar());
```

This tells the container to inject a new **Bar** instance whenever it sees an **IBar** constructor dependency.


#### Explicit service registration ####

Registers a service by providing explicit information about how to create the service instance and how to resolve the constructor dependencies.
```c#
container.Register<IBar, Bar>();
container.Register<IFoo>(factory => new Foo(factory.GetInstance<IBar>()));
var foo = (Foo)container.GetInstance<IFoo>();
Assert.IsNotNull(foo.Bar);
```

#### Parameters ####

Parameters are used when we want to supply one or more values when the service is resolved.

```c#
public class Foo : IFoo
{
    public Foo(int value)
    {
        Value = value;
    }

    public int Value { get; private set; }
}   
```

---

```c#
container.Register<int, IFoo>((factory, arg) => new Foo(arg));
var foo = (Foo)container.GetInstance<int, IFoo>(42);
Assert.AreEqual(42,foo.Value);
```

We can also do a combination of supplied values and dependencies.

```c#
public class Foo : IFoo
{
    public Foo(int value, IBar bar)
    {
        Value = value;
    }

    public int Value { get; private set; }
    public IBar Bar { get; private set; }
}    
```

---

```c#
container.Register<IBar, Bar>();
container.Register<int, IFoo>((factory, value) => new Foo(value, factory.GetInstance<IBar>()));
var foo = (Foo)container.GetInstance<int, IFoo>(42);
Assert.AreEqual(42, foo.Value);
Assert.IsNotNull(foo.Bar);
```

#### Optional arguments

LightInject will allow for default values to be used when a constructor dependency cannot be resolved.

```c#
public class Foo
{
    public Foo(string value = "42")
    {
        Value = value;
    }

    public string Value { get; }
}
```

We can still resolve `Foo` even though we have not registered a `string` service. 

```c#
var container = new ServiceContainer(options => options.EnableOptionalArguments = true);
container.Register<Foo>();
var instance = container.GetInstance<Foo>();
Assert.AreEqual("42", instance.Value)
```

> Note that the use cases for optional dependencies should be rare and are to be used with caution.



### Property Injection ###

```c#
public interface IFoo {}

public interface IBar {}

public class Foo : IFoo
{
    public IBar Bar { get; set; }
}

public class Bar : IBar {}
```

#### Implicit service registration ####

Registers the service without specifying any information about how to resolve the property dependencies.

```c#
container.Register<IFoo, Foo>();
container.Register<IBar, Bar>();
var foo = (Foo)container.GetInstance<IFoo>();
Assert.IsNotNull(foo.bar);
```

>**Note:** ***LightInject** considers all read/write properties a dependency, but implements a loose strategy around property dependencies, meaning that it will **NOT** throw an exception in the case of an unresolved property dependency.*          

For fine grained control of the injected property dependencies, we can provide a factory that makes it possible to create an instance of a given property dependency.

```c#
container.RegisterPropertyDependency<IBar>((factory, propertyInfo) => new Bar());
```

This tells the container to inject a new **Bar** instance whenever it sees an **IBar** property dependency.


#### Explicit service registration ####

Registers a service by providing explicit information about how to create the service instance and how to resolve the property dependencies.


```c#
container.Register<IBar, Bar>();
container.Register<IFoo>(factory => new Foo() {Bar = factory.GetInstance<IBar>()}) 
var foo = (Foo)container.GetInstance<IFoo>();
Assert.IsNotNull(foo.bar);
```

#### Property injection on existing instances. ####

In the cases where we don't control the creation of the service instance, **LightInject** can inject property dependencies into an existing instance.

```c#
container.Register<IBar, Bar>();
var foo = new Foo();
container.InjectProperties(foo);
Assert.IsNotNull(foo);
```

#### Disabling ProperyInjection

Property injection is enabled by default in **LightInject**, but it can be disabled like this.

```c#
var container = new ServiceContainer(new ContainerOptions { EnablePropertyInjection = false });
```

> It is actually recommended to turn off property injection unless it is really needed. Backward compatibility is the only reason that this is not the default.

## Initializers ##

Use the **Initialize** method to perform service instance initialization/post-processing.  

```c#
container.Register<IFoo, FooWithPropertyDependency>();
container.Initialize(
    registration => registration.ServiceType == typeof(IFoo), 
    (factory, instance) => ((FooWithPropertyDependency)instance).Bar = new Bar());
var foo = (FooWithProperyDependency)container.GetInstance<IFoo>();
Assert.IsInstanceOfType(foo.Bar, typeof(Bar));
```

## Assembly Scanning ##

LightInject is capable of registering services by looking at the types of a given assembly.

```c#
container.RegisterAssembly(typeof(IFoo).Assembly)
```

To filter out the services to be registered with the container, we can provide a predicate that makes it possible to inspect the service type and the implementing type.

```c#
container.RegisterAssembly(typeof(IFoo).Assembly, (serviceType, implementingType) => serviceType.NameSpace == "SomeNamespace");
```

It is also possible to scan a set assembly files based on a search pattern.

```c#
container.RegisterAssembly("SomeAssemblyName*.dll");  
```
When scanning assemblies, **LightInject** will register services using a service name that by default is the implementing type name. This behavior can be changed by specifying a function delegate to provide the name based on the service type and the implementing type.

```c#
container.RegisterAssembly(typeof(IFoo).Assembly, () => new PerContainerLifetime(), (serviceType, implementingType) => serviceType.NameSpace == "SomeNamespace", (serviceType, implementingType) => "Provide custom service name here");
```

We can also change this behavior globally for all registrations by implementing the **IServiceNameProvider** interface.

```c#
public class CustomServiceNameProvider : IServiceNameProvider
{
    public string GetServiceName(Type serviceType, Type implementingType)
    {
        return "Provide custom service name here";  
    }
}
```

To change the default behavior for all registrations we simply change this dependency on the container before we start scanning assemblies.

```c#
container.ServiceNameProvider = new CustomServiceNameProvider();
```



## Composition Root ##

When **LightInject** scans an assembly it will look for an implementation of the **ICompositionRoot** interface.   

```c#
public class SampleCompositionRoot : ICompositionRoot
{               
    public void Compose(IServiceRegistry serviceRegistry)
    {     
        serviceRegistry.Register(typeof(IFoo),typeof(Foo));
    }
}
```

If one or more implementations of the **ICompositionRoot** interface is found, they will be created and executed.

>**Note:** *Any other services contained within the target assembly that is not registered in the composition root, will **NOT** be registered.*

Rather that having a single composition root that basically needs to reference all other assemblies, having multiple composition roots makes it possible to group services naturally together. Another advantage of registering services in a **ICompositionRoot**, is that they can easily be reused in automated tests.   

### Lazy Composition Roots ###

**LightInject** is capable of registering services on a need to have basis. For a large application that has a lot of services, it might not be the best solution to register all these services up front as this could seriously hurt the startup time of our application due to extensive assembly loading.

If an unregistered service is requested, **LightInject** will scan the assembly where this service is contained.  

### CompositionRootAttribute ###

When an assembly is being scanned, **LightInject** will look for implementations of the **ICompositionRoot** interface. For large assemblies that contains many type, this might be an expensive operation. The **CompositionRootAttribute** is an assembly level attribute that simply helps **LightInject** to locate the compostion root.

```c#
[assembly: CompositionRootType(typeof(SampleCompositionRoot))]
```


### RegisterFrom ###

Allows explicit execution of a composition root.

```c#
container.RegisterFrom<SampleCompositionRoot>();
```

Alternatively we can also pass an existing composition root instance.

```c#
container.RegisterFrom(new SampleCompositionRoot());
```

## Generics ##

```c#
public interface IFoo<T> {};
public class Foo<T> : IFoo<T> {};
```

The container creates the closed generic type based on the service request.

```c#
container.Register(typeof(IFoo<>), typeof(Foo<>));
var instance = container.GetInstance(typeof(IFoo<int>));
Assert.IsInstanceOfType(instance, typeof(Foo<int>));
```

### Constraints ###

**LightInject** enforces generic constrains  


## Lazy&lt;T&gt; ##

**LightInject** can resolve a service as an instance of [Lazy&lt;T&gt;](http://msdn.microsoft.com/en-us/library/dd642331.aspx) when we want to postpone resolving the underlying service until it is needed.

```c#
public interface IFoo {}
public class Foo : IFoo {}
```

---

```c#
container.Register<IFoo, Foo>();
var lazyFoo = container.GetInstance<Lazy<IFoo>>();
Assert.IsNotNull(lazyFoo.Value);
```

## Function Factories ##

Function factories allows services to resolved as a function delegate that in turn is capable of returning the underlying service instance. We can think of this as an alternative to the [Service Locator](http://en.wikipedia.org/wiki/Service_locator_pattern) (anti)pattern.

```c#
public interface IFoo {}
public class Foo : IFoo {}
```

---

```c#
container.Register<IFoo,Foo>();
var func = container.GetInstance<Func<IFoo>>();
var foo = func();
Assert.IsNotNull(foo); 
```

>**Note:** *A function factory is effectively a delegate that redirects back to the corresponding **GetInstance** method on the service container.*

### Named Factories ###

The container returns a function delegate that represents calling the **GetInstance** method with "SomeFoo" as the service name argument.

```c#
container.Register<IFoo, Foo>("SomeFoo");
var func = container.GetInstance<Func<IFoo>>("SomeFoo");   
var foo = func();
Assert.IsNotNull(foo);
```


### Parameters ###

Function factories can also take parameters that will be used create the service instance.

```c#
public class Foo : IFoo
{
    public Foo(int value)
    {
        Value = value;
    }

    public int Value { get; private set; }
}
```

---

```c#
container.Register<int, IFoo>((factory, value) => new Foo(value));
var fooFactory = container.GetInstance<Func<int, IFoo>>();
var foo = (Foo)fooFactory(42); 
Assert.AreEqual(foo.Value, 42);
```

>**Note** : *The service must be explicitly registered in order for the container to resolve it as a parameterized function factory.*

### IDisposable ###

The only way to deal with disposable objects when using function factories, is to let the service type inherit from IDisposable.

```c#
public interface IFoo : IDisposable {}
public class Foo : IFoo {}
```

---

```c#
container.Register<IFoo, Foo>();
var fooFactory = container.GetInstance<Func<IFoo>>();

using(IFoo foo = fooFactory())
{
    
} <--Instance is disposed here          
```

>**Note:** *Although this is common practice even in the [BCL](http://en.wikipedia.org/wiki/Base_Class_Library), this kind of interfaces are often referred to as [leaky abstractions](http://en.wikipedia.org/wiki/Leaky_abstraction).*

## Typed Factories  ##

A typed factory is a class that wraps the function factory that is used to create the underlying service instance.
As opposed to just function factories, typed factories provides better expressiveness to the consumer of the factory.  

```c#
public interface IFooFactory
{
    IFoo GetFoo();
}
```

---

```c#
public class FooFactory : IFooFactory
{
    private Func<IFoo> createFoo;

    public FooFactory(Func<IFoo> createFoo)
    {
        this.createFoo = createFoo;
    }

    public IFoo GetFoo()
    {
        return createFoo();
    }
} 
```

---

```c#
container.Register<IFoo, Foo>();
container.Register<IFooFactory, FooFactory>(new PerContainerLifetime());
var fooFactory = container.GetInstance<IFooFactory>();
var foo = fooFactory.GetFoo();
Assert.IsNotNull(foo);
```

>**Note:** *Register typed factories with the **PerContainerLifetime** unless a compelling reason exists to choose a different lifetime.*  

### Parameters ###

Types factories can also wrap a parameterized function factory and allows us to pass arguments.

```c#
public class Foo : IFoo
{
    public Foo(int value)
    {
        Value = value;
    }

    public int Value { get; private set; }
}

public interface IFooFactory
{
    IFoo GetFoo(int value);
} 
```

---

```c#
public class FooFactory : IFooFactory
{
    private Func<int, IFoo> createFoo;

    public FooFactory(Func<int, IFoo> createFoo)
    {
        this.createFoo = createFoo;
    }

    public IFoo GetFoo(int value)
    {
        return createFoo(value);
    }
} 
```

---

```c#
container.Register<int, IFoo>((factory, value) => new Foo(value));
container.Register<IFooFactory, FooFactory>(new PerContainerLifetime());
var typedFooFactory = container.GetInstance<IFooFactory>();
var foo = typedFooFactory.GetFoo(42);
Assert.AreEqual(foo.Value, 42);
```

### IDisposable ###

Working with typed factories gives us the possibility to release disposable services registered as transients without exposing a leaky abstraction.

```c#
public interface IFooFactory
{
    IFoo GetFoo(int value);
    void Release(IFoo foo);
} 
```

---

```c#
public class FooFactory : IFooFactory
{
    private Func<IFoo> createFoo;

    public FooFactory(Func<IFoo> createFoo)
    {
        this.createFoo = createFoo;
    }

    public IFoo GetFoo(int value)
    {
        return createFoo(value);
    }

    public void Release(IFoo foo)
    {
        var disposable = foo as IDisposable;
        if (disposable != null)
        {
            disposable.Dispose();
        }
    }
}    
```

## Recursive dependency detection ##

A recursive dependency graph is when a service depends directly or indirectly on itself.

```c#
public class FooWithRecursiveDependency : IFoo
{
    public FooWithRecursiveDependency(IFoo foo)
    {
    }
}
```

The following code will throw an **InvalidOperationException** stating that there are existing recursive dependencies. 

```c#
container.Register(typeof(IFoo), typeof(FooWithRecursiveDependency));
container.GetInstance<IFoo>()
```

## Internals ##

When running under the .Net platform, **LightInject** is capable of creating instances of classes that has the [internal](http://msdn.microsoft.com/en-us/library/7c5ka91b.aspx) modifier. 

The only requirement is that the internal class exposes a public constructor.

```c#
internal class InternalFooWithPublicConstructor : IFoo
{
    public InternalFooWithPublicConstructor () {}
}
```

## Logging ##

Sometimes it might be useful to obtain information about what is going on inside the container 
and **LightInject** provides a very simple log abstraction that is used to log information and warnings from within the container.

```csharp
var containerOptions = new ContainerOptions();
containerOptions.LogFactory = (type) => logEntry => Console.WriteLine(logEntry.Message);
```

## Unit Testing

Sometimes it might be useful to use the service container within our unit tests. LightInject also provides the [LightInject.xUnit](https://www.nuget.org/packages/LightInject.xUnit/) extension that enables dependencies to be injected into test methods. One side effect of using that extension is that it is tightly coupled to xUnit and it we have less control with regards to container instances. 

Instead consider this simple base class 

```c#
public class ContainerFixture : IDisposable
{
    public ContainerFixture()
    {
        var container = CreateContainer();
        Configure(container);
        container.RegisterFrom<CompositionRoot>();
        ServiceFactory = container.BeginScope();
        InjectPrivateFields();
    }

    private void InjectPrivateFields()
    {
        var privateInstanceFields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var privateInstanceField in privateInstanceFields)
        {
            privateInstanceField.SetValue(this, GetInstance(ServiceFactory, privateInstanceField));
        }
    }

    internal Scope ServiceFactory { get; }

    public void Dispose() => ServiceFactory.Dispose();

    public TService GetInstance<TService>(string name = "")
        => ServiceFactory.GetInstance<TService>(name);

    private object GetInstance(IServiceFactory factory, FieldInfo field)
        => ServiceFactory.TryGetInstance(field.FieldType) ?? ServiceFactory.GetInstance(field.FieldType, field.Name);

    internal virtual IServiceContainer CreateContainer() => new ServiceContainer();

    internal virtual void Configure(IServiceRegistry serviceRegistry) {}
}
```



This can be use with any test framework as long as it creates a new instance of the test class for each test method and that it calls `Dispose` after the test completes. For `xUnit` this is the default behaviour.

Injecting services now becomes incredible easy. Just declare the service to test as a private field like this.

```c#
public class SampleTests : ContainerFixture
{
    private ICalculator calculator;
    
    [Fact]
    public void ShouldAddNumbers()
    {
        calculator.Add(2,2).ShouldBe(2);
    }
}
```

If we need to configure the container before executing the test, we can do that by simply overriding the `Configure` method. This could for instance be used to register mock services into the container.

```c#
public class SampleTests : ContainerFixture
{
    private ICalculator calculator;
    
    [Fact]
    public void ShouldAddNumbers()
    {
        calculator.Add(2,2).ShouldBe(2);
    }
    
    internal override Configure(IServiceRegistry serviceRegistry)
    {
        // Add registrations related to testing here
    }
}
```













