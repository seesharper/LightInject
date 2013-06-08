## Installing ##

Getting started with **LightInject** is as easy as getting the **LightInject** package from NuGet.

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject </code>
   </p>
</div>


This will install a single file (servicecontainer.cs) into the current project.

## Services ##

We can start using LightInject without registering any services.

	var container = new ServiceContainer();
    IFoo foo = container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(foo, typeof(Foo));
    
**Note:** *If no services are registered, **LightInject** will scan the containing assembly upon the first request.*


##Service Registration##


### Basic type mapping ###

Create mapping between the service type and the implementing type.  

	container.Register<IFoo,Foo>();


#### Named Services ####

We can register multiple services under the same servicetype using named services. 

	container.Register<IFoo, Foo>();
	container.Register<IFoo, AnotherFoo>("SomeFoo");

####Values###

	var foo = new Foo();
	container.Register<IFoo>(foo);

The value is registered as a constant value .

####Concrete types###

**LightInject** allows concrete types to be registered as a service.

	container.Register<Foo>();

####Unknown types###

We can register a predicate along with a factory delegate to deal with creating instances of types that 
are not known by the service container.

    container.Register((serviceType, serviceName) => serviceType == typeof(IFoo), request => new Foo());
    var instance = container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance, typeof(IFoo));

We can also specify the lifetime for unknown types.

	container.Register((serviceType, serviceName) => serviceType == typeof(IFoo), request => new Foo(), new PerContainerLifetime());
	var firstInstance = container.GetInstance<IFoo>();
	var secondInstance = container.GetInstance<IFoo>();
	Assert.AreSame(firstInstance, secondInstance);


###Dependencies ###

LightInject provides two patterns for registering services which we will refer to as **implicit** and **explicit** service registration.

### Constructor Injection ###

	public interface IFoo {}

	public interface IBar {}

	public class FooWithDependency : IFoo
	{
		public FooWithDependency(IBar bar){}
 	}

	public class Bar : IBar {}


####Implicit service registration####

	container.Register<IFoo, FooWithDependency>();
	container.Register<IBar, Bar>();

Registers a service without specifying any information about how to resolve the constructor dependencies of the implementing type.
	
####Explicit service registration####

	container.Register<IBar, Bar>();
	container.Register<IFoo>(factory => new FooWithDependency(factory.GetInstance<IBar>())) 

Registers a service by providing explicit information about how to create the service instance and how to resolve the constructor dependencies.

### Property Injection ###


	public interface IFoo {}

	public interface IBar {}

	public class FooWithPropertyDependency : IFoo
	{
		public IBar Bar { get; set; }
 	}

	public class Bar : IBar {}
	
####Implicit service registration####

	container.Register<IFoo, FooWithPropertyDependency>();
	container.Register<IBar, Bar>();

Registers the service without specifying any information about how to resolve the property dependencies. 

**Note:** ***LightInject** implements a loose strategy around property dependencies, meaning that it will **NOT** throw an exception in the case of an unresolved property dependency.*

####Explicit service registration####

	container.Register<IBar, Bar>();
	container.Register<IFoo>(factory => new FooWithPropertyDependency() {Bar = factory.GetInstance<IBar>()}) 

Registers a service by providing explicit information about how to create the service instance and how to resolve the property dependencies.

#### Property injection on existing instances. ####

In the cases where we don't control the creation of the service instance, **LightInject** can inject property dependencies into an existing instance.

	container.Register<IBar, Bar>();
	var fooWithProperyDependency = new FooWithProperyDependency();
    var result = (FooWithProperyDependency)container.InjectProperties(fooWithProperyDependency);
    Assert.IsInstanceOfType(result.Bar, typeof(Bar));

If we want to be explicit about the dependencies injected into the service instance, the concrete type can be registered with the container.

	container.Register<IBar, Bar>();
    container.Register<IBar, AnotherBar>("AnotherBar");
    container.Register(f => new FooWithProperyDependency(){ Bar = f.GetInstance<IBar>("AnotherBar") });
    var fooWithProperyDependency = new FooWithProperyDependency();
    var result = (FooWithProperyDependency)container.InjectProperties(fooWithProperyDependency);
    Assert.IsInstanceOfType(result.Bar, typeof(AnotherBar));



### Assembly  ###

---
	container.RegisterAssembly(typeof(IFoo).Assembly)

Registers all types within an assembly.

	container.RegisterAssembly(typeof(IFoo).Assembly, type => type.NameSpace == "SomeNamespace");

Registers all types within an assembly and uses the given predicate to decide if the current implementing type should be registered with the container. 

	container.RegisterAssembly("SomeAssemblyName*.dll");

Registers services from all assemblies that matches the given search pattern.

#### CompositionRoot ####

When working with modular applications, it might be necessary to allow the modules to register services with the service container. This can be done by implementing the **ICompositionRoot** interface.   

    public class SampleCompositionRoot : ICompositionRoot
    {               
        public void Compose(IServiceRegistry serviceRegistry)
        {     
            serviceRegistry.Register(typeof(IFoo),typeof(Foo));
        }
    }

When we register an assembly, the container will first look for implementations of the **ICompositionRoot** interface. If one or more implementations are found, they will be created and invoked.

**Note:** Any other services contained within the target assembly that is not registered in the composition root, will **NOT** be registered.

### Recursive dependency detection ###

A recursive dependency graph is when a service depends directly or indirectly on itself.

    public class FooWithRecursiveDependency : IFoo
    {
        public FooWithRecursiveDependency(IFoo foo)
        {
        }
    }

The following code will throw an **InvalidOperationException** stating that there are existing recursive dependencies. 

	container.Register(typeof(IFoo), typeof(FooWithRecursiveDependency));
	container.GetInstance<IFoo>()


## Service Resolution

### GetInstance ###

The service container will throw an **InvalidOperationException** if the service container is unable to resolve the service or any of its dependencies.

	container.Register<IFoo, Foo>();	
	ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IBar>());


### TryGetInstance ###

The service container will **NOT** throw an exception when resolving an unknown service.
	
	container.Register<IBar, Bar>();	
	var instance = container.TryGetInstance<IFoo>();	
	Assert.IsNull(instance);


The container will still throw an exception for a known service resolved via the **TryGetInstance** method if any of its dependencies can not be resolved.

	var container = CreateContainer();
	container.Register<IFoo, FooWithDependency>();	
	ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());


### Basic Types ###

	public interface IFoo {}:
	public class Foo {}:

The container resolves the instance based on the requested service type.

	container.Register<IFoo,Foo>();	
	var instance = container.GetInstance<IFoo>();
	Assert.IsInstanceOfType(instance, typeof(Foo));



### Open Generic Types ###

	public interface IFoo<T> {};
	public class Foo<T> : IFoo<T> {};
		
The container creates the closed generic type based on the service request.
	 
    container.Register(typeof(IFoo<>), typeof(Foo<>));
    var instance = container.GetInstance(typeof(IFoo<int>));
    Assert.IsInstanceOfType(instance, typeof(Foo<int>));

### Func&lt;T&gt; ###

    public class FooWithFuncDependency : IFoo
    {
        public FooWithFuncDependency(Func<IBar> getBar)
        {
            GetBar = getBar;
        }
        public Func<IBar> GetBar { get; private set; } 
    }

The container creates a delegate that is capable of resolving the underlying dependency (IBar).

	container.Register<IBar, Bar>();
	container.Register<IFoo, FooWithFuncDependency>();
	var instance = (FooWithFuncDependency)container.GetInstance<IFoo>();
	Assert.IsInstanceOfType(instance.GetBar(), typeof(Bar));

### Func&lt;string,T&gt; ###

    public class FooWithNamedFuncDependency : IFoo
    {
        public FooWithNamedFuncDependency(Func<string, IBar> getBar)
        {
            GetBar = getBar;
        }
        public Func<string, IBar> GetBar { get; private set; }
    }

The container creates a delegate that is capable of resolving the underlying named dependency.

    var container = CreateContainer();
    container.Register<IBar, Bar>("SomeBar");
    container.Register<IFoo, FooWithNamedFuncDependency>();
    var instance = (FooWithNamedFuncDependency)container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance.GetBar("SomeBar"), typeof(Bar));


### IEnumerable&lt;T&gt; ###

	public interface IFoo {};
	public class Foo : IFoo {};
	public class AnotherFoo : IFoo {};

The container resolves all instances as an **IEnumerable&lt;T&gt;**.

	container.Register(typeof(IFoo), typeof(Foo));
	container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
	var services = container.GetInstance<IEnumerable<IFoo>>();
	Assert.AreEqual(2, services.Count());

Alternatively using the **GetAllInstances** method.

	container.Register(typeof(IFoo), typeof(Foo));
	container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
	var services = container.GetAllInstance<<IFoo>();
	Assert.AreEqual(2, services.Count());


## IDisposable ##


The recommened way of dealing with a disposable object is to wrap it in a using block.

	using(IFoo disposableFoo = new DisposableFoo())
	{

	} <--the instance is disposed here.

The **using** block defines a scope for the instance and the instance is safely disposed when the scope ends.

**Note:** 
*We might argue that the **using** block invites to [leaky abstractions](http://en.wikipedia.org/wiki/Leaky_abstraction "leaky abstractions") as the abstraction(**IFoo**) is required to "implement" the **IDisposable** interface in order for this code to compile.*

**LightInject** offers the **BeginScope** method that defines a new scope similar to the **using** block. 


	using(var scope = container.BeginScope())
	{
		IFoo instance = container.GetInstance<IFoo>(); 
	} //instance is disposed here.

Scopes can also be nested 

	using(var outerScope = container.BeginScope())
	{
		IFoo outerInstance = container.GetInstance<IFoo>(); 
		using(var innerScope = container.BeginScope())
		{
			IFoo innerInstance = container.GetInstance<IFoo>(); 
		} //innerInstance is disposed here.
	} //outerInstance is disposed here.


##Lifetime##

The default behavior in **LightInject** is to treat all objects as transients unless otherwise specified.

	container.Register<IFoo,Foo>();
	var firstInstance = container.GetInstance<IFoo>();
	var secondInstance = container.GetInstance<IFoo>();
	Assert.AreNotSame(firstInstance, secondInstance);

###PerScopeLifetime###

Ensures that only one instance of a given service can exists within a scope.
The container will call the **Dispose** method on all disposable objects created within the scope.

	container.Register<IFoo,Foo>(new PerScopeLifetime());
	using(container.BeginScope())
	{
		
		var firstInstance = container.GetInstance<IFoo>();
		var secondInstance = container.GetInstance<IFoo>();
		Assert.AreSame(firstInstance, secondInstance);
	}

**Note:** *An **InvalidOperationException** is thrown if a service registered with the **PerScopeLifetime** is requested outside the scope.*

###PerContainerLifetime###

Ensures that only one instance of a given service can exist within the container.
The container will call the Dispose method on all disposable objects when the container itself is disposed.

	using(container = new ServiceContainer())
	{
		container.Register<IFoo,Foo>(new PerContainerLifetime());	
		var firstInstance = container.GetInstance<IFoo>();
		var secondInstance = container.GetInstance<IFoo>();
		Assert.AreSame(firstInstance, secondInstance);
	}

###PerRequestLifetime###

A new instance is created for each request and the container calls **Dispose** when the scope ends.
This lifetime is used when the conrete class implements **IDisposable**.

	container.Register<IFoo,Foo>(new PerRequestLifetime());
	using(container.BeginScope())
	{		
		var firstInstance = container.GetInstance<IFoo>();
		var secondInstance = container.GetInstance<IFoo>();
		Assert.AreNotSame(firstInstance, secondInstance);
	}	

**Note:** *An **InvalidOperationException** is thrown if a service registered with the **PerRequestLifetime** is requested outside the scope.*


###Custom lifetime###

A custom lifetime is created by implementing the **ILifetime** interface

    internal interface ILifetime
    {
        object GetInstance(Func<object> instanceFactory, Scope currentScope);        
    }

The following example shows to create a custom lifetime that ensures only one instance per thread.

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
	
That is all it takes to create a custom lifetime, but what about disposable services?

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
						throw new InvalidOperationException("Attempt to create an disposable object 
															without a current scope.")
					}
					currentScope.TrackInstance(disposable);
				}

				instances.value = instance;
			}
			return instance.value;
		}
	}






####Important####
 		
A lifetime object controls the lifetime of a single service and can **never** be shared for multiple service registrations.

**Wrong**

	ILifetime lifetime = new PerContainerLifeTime();
	container.Register<IFoo,Foo>(lifetime);
	container.Register<IBar,Bar>(lifetime);

**Right**

	container.Register<IFoo,Foo>(new PerContainerLifeTime());
	container.Register<IBar,Bar>(new PerContainerLifeTime());

A lifetime object is also shared across threads and that is something we must take into consideration when developing new lifetime implementations.
