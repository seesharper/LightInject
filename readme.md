# LightInject #

**LightInject** is an ultra lightweight IoC container that supports the most common features expected from a service container.

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject </code>
   </p>
</div>

This will install a single file named "ServiceContainer.cs" into the current project.

## The Rationale Behind ##


**LightInject** was created to provide a very simple, super fast and easy-to-learn service container that can be used in small projects as well as part of larger applications.

**LightInject** is specifically designed not to bleed into the application code and thus creating a dependency to the container everywhere in the application.

**LightInject** is also very well suited for stand alone class libraries that requires a service container without creating a dependency to a third-party assembly. We can keep the container within our class library and still ship the library as a single DLL.

**LightInject** uses Reflection.Emit to dynamically create the code needed to resolve services and dependencies and thus providing performance numbers very close to the new operator.

## The code ##

The first thing to notice about the code is that every type that is expected to be **public**, is marked with the **internal** access modifier.

This is by design to prevent the service container to leak out into the application code and hence creating a dependency to a spesific service container implementation.

If we need to expose any of the types within **LightInject** to the outside world, we need to use the [InternalVisibleTo](http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.internalsvisibletoattribute(v=vs.100).aspx) attribute.


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

###Dependencies ###

LightInject provides two patterns for registering services which we will refer to as **implicit** and **explicit** service registration.

### Constructor Injection ###
----
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
----
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

####Explicit service registration####

	container.Register<IBar, Bar>();
	container.Register<IFoo>(factory => new FooWithPropertyDependency() {Bar = factory.GetInstance<IBar>()}) 

Registers a service by providing explicit information about how to create the service instance and how to resolve the property dependencies.


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

###GetInstance ###

The service container will throw an **InvalidOperationException** if the service container is unable to resolve the service or any of its dependencies.

	container.Register<IFoo, Foo>();	
	ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IBar>());


###TryGetInstance###

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


##IDisposable##

----------


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

##Decorators##

**LightInject** has native support for the [decorator pattern](http://en.wikipedia.org/wiki/Decorator_pattern).

A decorator is a class that implements the same interface as the type it is decorating and takes the target instance as a constructor argument.

	public class FooDecorator : IFoo
	{     
	    public FooDecorator(IFoo foo)
	    {     
	    }
	} 
	

Decorators are applied using the **Decorate** method.	
    
	container.Register<IFoo, Foo>();
    container.Decorate(typeof(IFoo), typeof(FooDecorator));

    var instance = container.GetInstance<IFoo>();

    Assert.IsInstanceOfType(instance, typeof(FooDecorator));

Decorators can be nested and they are applied in the same sequence as they are registered.

    container.Register<IFoo, Foo>();            
    container.Decorate(typeof(IFoo), typeof(FooDecorator));
    container.Decorate(typeof(IFoo), typeof(AnotherFooDecorator));

    var instance = container.GetInstance<IFoo>();

    Assert.IsInstanceOfType(instance, typeof(AnotherFooDecorator));

If we have multiple services implementing the same interface, we can apply the decorator to implementations matching the given predicate.
    
    container.Register<IFoo, Foo>();
    container.Register<IFoo, AnotherFoo>("AnotherFoo");
    container.Decorate(typeof(IFoo), typeof(FooDecorator), service => service.ServiceName == "AnotherFoo");

    var instance = container.GetInstance<IFoo>();
    var decoratedInstance = container.GetInstance<IFoo>("AnotherFoo");

    Assert.IsInstanceOfType(instance, typeof(Foo));
    Assert.IsInstanceOfType(decoratedInstance, typeof(FooDecorator));

Decorators can have their own dependencies in addition to the target instance it is decorating.

    public class FooDecoratorWithDependency : IFoo
    {        
        public FooDecoratorWithDependency(IFoo foo, IBar bar)
        {
            Foo = foo;
            Bar = bar;
        }

        public IFoo Foo { get; private set; }

        public IBar Bar { get; private set; }
    }

The dependencies of the decorator can be implicitly resolved.

	container.Register<IFoo, Foo>();
	container.Register<IBar, Bar>();
	container.Decorate(typeof(IFoo), typeof(FooDecoratorWithDependency));
	var instance = (FooDecoratorWithDependency)container.GetInstance<IFoo>();
	Assert.IsInstanceOfType(instance.Foo, typeof(IFoo));
	Assert.IsInstanceOfType(instance.Bar, typeof(IBar));


By using a function factory, we can explicitly specify the depenendecies of the decorator.

    container.Register<IFoo, Foo>();
    container.Register<IBar, Bar>();
    container.Decorate<IFoo>((serviceFactory, target) 
        => new FooDecoratorWithDependency(target, serviceFactory.GetInstance<IBar>()));
    var instance = (FooDecoratorWithDependency)container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance.Foo, typeof(IFoo));
    Assert.IsInstanceOfType(instance.Bar, typeof(IBar));

**Note:** *The target instance is available through the function delegate so that we can pass it to the constructor of the decorator.*


Decorators can also be applied to open generic types.

    container.Register(typeof(IFoo<>), typeof(Foo<>));
    container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>));

    var instance = container.GetInstance<IFoo<int>>();

    Assert.IsInstanceOfType(instance, typeof(FooDecorator<int>));

## Composite pattern ##

The [composite pattern](http://en.wikipedia.org/wiki/Composite_pattern) is a simple pattern that lets a class implement an interface and then delegates invocation of methods to a set other classes implementing the same interface. 



    public class FooWithEnumerableIFooDependency : IFoo
    {
        public IEnumerable<IFoo> FooList { get; private set; }

        public FooWithEnumerableIFooDependency(IEnumerable<IFoo> fooList)
        {
            FooList = fooList;
        }
    }

While this looks like a recursive dependency, **LightInject** detects this and removes the  **FooWithEnumerableIFooDependency** from the IEnumerable&lt;IFoo&gt; beeing injected.	 

    container.Register(typeof(IFoo), typeof(Foo), "Foo");
    container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
    container.Register(typeof(IFoo), typeof(FooWithEnumerableIFooDependency));            
    var instance = (FooWithEnumerableIFooDependency)container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance.FooList.First(), typeof(Foo));
    Assert.IsInstanceOfType(instance.FooList.Last(), typeof(AnotherFoo));

##Unit testing##

**LightInject** provides native support for mocking services during unit testing.

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Mocking </code>
   </p>
</div>

**Note:** *Use the [InternalVisibleTo](http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.internalsvisibletoattribute(v=vs.100).aspx) attribute to give the test project access to the **LightInject** internal types.*

	public class FooWithDependency : IFoo
	{
		public FooWithDependency(IBar bar)
		{
		}
	}


The container is configured as usual at the composition root.

	container.Register<IFoo,FooWithDependency>();
	container.Register<IBar,Bar>();

Now, in our unit test project we would like to write tests against the **IFoo** service, but we would also like to replace the dependency (**Bar**) with a mock instance.

The following example uses the [Moq](http://code.google.com/p/moq/) library to provide a mock instance for the **IBar** dependency.

	barMock = new Mock<IBar>();
	container.StartMocking<IBar>(() => barMock.Object);	

	var foo = (FooWithDependency)container.GetInstance<IFoo>();

	Assert.IsNotInstanceOfType(foo.Bar, typeof(Bar));
	container.EndMocking<IBar>()

When the **StartMocking** method is called, the container will replace the original service registration with a new service registration that uses our mock instance. 

**Note:** *The mock instance uses the same lifetime as the original registration.*

The **StopMocking** method tells the container to replace the mock registration with our original service registration.

	barMock = new Mock<IBar>();
	container.StartMocking<IBar>(() => barMock.Object);	
	container.EndMocking<IBar>();
	
	var foo = (FooWithDependency)container.GetInstance<IFoo>();

	Assert.IsInstanceOfType(foo.Bar, typeof(Bar));
	

	





