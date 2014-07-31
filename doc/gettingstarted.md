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


### Default services###

    public interface IFoo {}
    public class Foo : IFoo {}

---

    container.Register<IFoo, Foo>();
    var instance = container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance, typeof(Foo));

### Named services ###
    
    public class Foo : IFoo {}
    public class AnotherFoo : IFoo {}
    
---

    container.Register<IFoo, Foo>();
    container.Register<IFoo, AnotherFoo>("AnotherFoo");
    var instance = container.GetInstance<IFoo>("AnotherFoo");
    Assert.IsInstanceOfType(instance, typeof(AnotherFoo));

If only one named registration exists, **LightInject** is capable of resolving this as the default service.
    
    container.Register<IFoo, AnotherFoo>("AnotherFoo");
    var instance = container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance, typeof(AnotherFoo));

### Unresolved services ###

LightInject can resolve services that are not registered with the container using the *RegisterFallback* method.

    var container = new ServiceContainer();
    container.RegisterFallback((type, s) => true, request => new Foo());
    var foo = container.GetInstance<IFoo>();

The first argument to the *RegisterFallback* method makes it possible to possible to decide if the service can be "late-resolved".
The second argument is a *ServiceRequest* instance that provides the requested service type and service name.


 


### IEnumerable&lt;T&gt; ###

When we register multiple services with the same service type, **LightInject** is capable of resolving these services as an  [IEnumerable&lt;T&gt;](http://msdn.microsoft.com/en-us/library/9eekhta0.aspx).

    public class Foo : IFoo {}
    public class AnotherFoo : IFoo {}
    
---

    container.Register<IFoo, Foo>();
    container.Register<IFoo, AnotherFoo>("AnotherFoo");
    var instances = container.GetInstance<IEnumerable<IFoo>>()
    Assert.AreEqual(2, instances.Count());

Alternatively using the **GetAllInstances** method.

    var instances = container.GetAllInstances<IFoo>();
    Assert.AreEqual(2, instances.Count());

In addition, **LightInject** supports the following [IEnumerable&lt;T&gt;](http://msdn.microsoft.com/en-us/library/9eekhta0.aspx) sub-types. 

* Array
* ICollection&lt;T&gt;
* IList&lt;T&gt;
* IReadOnlyCollection&lt;T&gt; (Net 4.5 and Windows Runtime);
* IReadOnlyList&lt;T&gt; (Net 4.5 and Windows Runtime)

### Values ###

Registers the value as a constant.

    container.RegisterInstance<string>("SomeValue");
    var value = container.GetInstance<string>();
    Assert.AreEqual("SomeValue, value);


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

>**Note:** *An **InvalidOperationException** is thrown if a service registered with the **PerRequestLifetime** is requested outside the scope.*


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

### Async and Await ###

By default scopes are managed per thread which means that when the container looks for the current scope, it will look for a scope that is associated with the current thread.

With the introduction of the async/await pattern chances are that the code that is requesting a service instance is running on another thread.

To illustrate this lets consider an example that is going to cause an instance to be resolved on another thread.

We start of by creating an interface that returns a **Task&lt;IBar&gt;**

    public interface IAsyncFoo
    {
        Task<IBar> GetBar();
    }

Next we implement this interface in such a way that the **IBar** instance is requested on another thread.

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
 
The we register the dependency (**IBar**) with the **PerScopeLifetime** that is going to cause the container to ask for the current scope so that the instance can be registered with that scope.

	var container = new ServiceContainer();
    container.Register<IBar, Bar>(new PerScopeLifetime());
    container.Register<IAsyncFoo, AsyncFoo>();

    using (container.BeginScope())
    {
        var instance = container.GetInstance<IAsyncFoo>();
        ExceptionAssert.Throws<AggregateException>(() => instance.GetBar().Wait());                
    }
  
This will throw an exception that states the following:

	Attempt to create a scoped instance without a current scope.  
 
The reason that this is happening is that the current scope is associated with the thread that created it and when the continuation executes, we are essentially requesting an instance on another thread.

To deal with this issue, **LightInject** now supports scopes across the logical [CallContext](http://msdn.microsoft.com/en-us/library/system.runtime.remoting.messaging.callcontext(v=vs.110).aspx).  

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

> Note that the **PerLogicalCallContextScopeManagerProvider** is only available when running under .Net 4.5.
> For more information, please refer to the following [article](http://blog.stephencleary.com/2013/04/implicit-async-context-asynclocal.html) by Stephen Cleary.


## Dependencies ##
 

### Constructor Injection ##
    
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

#### Implicit service registration ####

Registers a service without specifying any information about how to resolve the constructor dependencies of the implementing type.

    container.Register<IFoo, Foo>();
    container.Register<IBar, Bar>();
    var foo = (Foo)container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(foo.Bar, typeof(Bar)); 
     
> Note: In the case where the implementing type(Foo) has more than one constructor, **LightInject** will choose the constructor with the most parameters. 

#### Explicit service registration ####

Registers a service by providing explicit information about how to create the service instance and how to resolve the constructor dependencies.
    
    container.Register<IBar, Bar>();
    container.Register<IFoo>(factory => new Foo(factory.GetInstance<IBar>));
    var foo = (Foo)container.GetInstance<IFoo>();
    Assert.IsNotNull(foo.Bar);            

#### Parameters ####

Parameters are used when we want to supply one or more values when the service is resolved.

    public class Foo : IFoo
    {
        public Foo(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }   

---

    container.Register<int, IFoo>((arg, factory) => new Foo(arg));
    var foo = (Foo)container.GetInstance<int, IFoo>(42);
    Assert.AreEqual(42,foo.Value);

We can also do a combination of supplied values and dependencies.

    public class Foo : IFoo
    {
        public Foo(int value, IBar bar)
        {
            Value = value;
        }

        public int Value { get; private set; }
        public IBar Bar { get; private set; }
    }    

---

    container.Register<IBar, Bar>();
    container.RegisterFactory<int, IFoo>((factory, value) => new Foo(value, factory.GetInstance<IBar>()));
    var foo = (Foo)container.GetInstance<int, IFoo>();
    Assert.AreEqual(42, foo.Value);
    Assert.IsNotNull(foo.Bar);

### Property Injection ###

	public interface IFoo {}

	public interface IBar {}

	public class Foo : IFoo
	{
		public IBar Bar { get; set; }
 	}

	public class Bar : IBar {}

####Implicit service registration####

Registers the service without specifying any information about how to resolve the property dependencies.

    container.Register<IFoo, Foo>();
	container.Register<IBar, Bar>();
    var foo = (Foo)container.GetInstance<IFoo>();
    Assert.IsNotNull(foo.bar);

>**Note:** ***LightInject** considers all read/write properties a dependency, but implements a loose strategy around property dependencies, meaning that it will **NOT** throw an exception in the case of an unresolved property dependency.*          

####Explicit service registration####

Registers a service by providing explicit information about how to create the service instance and how to resolve the property dependencies.


	container.Register<IBar, Bar>();
	container.Register<IFoo>(factory => new Foo() {Bar = factory.GetInstance<IBar>()}) 
    var foo = (Foo)container.GetInstance<IFoo>();
    Assert.IsNotNull(foo.bar);

#### Property injection on existing instances. ####

In the cases where we don't control the creation of the service instance, **LightInject** can inject property dependencies into an existing instance.

	container.Register<IBar, Bar>();
	var foo = new Foo();
    container.InjectProperties(foo);
    Assert.IsNotNull(foo);

## Assembly Scanning ##

LightInject is capable of registering services by looking at the types of a given assembly.

    container.RegisterAssembly(typeof(IFoo).Assembly)

To filter out the services to be registered with the container, we can provide a predicate that makes it possible to inspect the service type and the implementing type.

	container.RegisterAssembly(typeof(IFoo).Assembly, (serviceType, implementingType) => serviceType.NameSpace == "SomeNamespace");
 
It is also possible to scan a set assembly files based on a search pattern.

    container.RegisterAssembly("SomeAssemblyName*.dll");  

 


## Composition Root ##

When **LightInject** scans an assembly it will look for an implementation of the **ICompositionRoot** interface.   

    public class SampleCompositionRoot : ICompositionRoot
    {               
        public void Compose(IServiceRegistry serviceRegistry)
        {     
            serviceRegistry.Register(typeof(IFoo),typeof(Foo));
        }
    }
 
If one or more implementations of the **ICompositionRoot** interface is found, they will be created and executed.

>**Note:** *Any other services contained within the target assembly that is not registered in the composition root, will **NOT** be registered.*

Rather that having a single composition root that basically needs to reference all other assemblies, having multiple composition roots makes it possible to group services naturally together. Another advantage of registering services in a **ICompositionRoot**, is that they can easily be reused in automated tests.   

### Lazy Composition Roots ###

**LightInject** is capable of registering services on a need to have basis. For a large application that has a lot of services, it might not be the best solution to register all these services up front as this could seriously hurt the startup time of our application due to extensive assembly loading.

If an unregistered service is requested, **LightInject** will scan the assembly where this service is contained.  

### CompositionRootAttribute ###

When an assembly is being scanned, **LightInject** will look for implementations of the **ICompositionRoot** interface. For large assemblies that contains many type, this might be an expensive operation. The **CompositionRootAttribute** is an assembly level attribute that simply helps **LightInject** to locate the compostion root.

    [assembly: CompositionRootType(typeof(SampleCompositionRoot))]


### RegisterFrom ###

Allows explicit execution of a composition root.

	container.RegisterFrom<SampleCompositionRoot>();


## Generics ##

	public interface IFoo<T> {};
	public class Foo<T> : IFoo<T> {};
		
The container creates the closed generic type based on the service request.
	 
    container.Register(typeof(IFoo<>), typeof(Foo<>));
    var instance = container.GetInstance(typeof(IFoo<int>));
    Assert.IsInstanceOfType(instance, typeof(Foo<int>));

### Constraints ###

**LightInject** enforces generic constrains  


## Lazy&lt;T&gt; ##

**LightInject** can resolve a service as an instance of [Lazy&lt;T&gt;](http://msdn.microsoft.com/en-us/library/dd642331.aspx) when we want to postpone resolving the underlying service until it is needed.

    public interface IFoo {}
    public class Foo : IFoo {}

---

    container.Register<IFoo, Foo>();
    var lazyFoo = container.GetInstance<Lazy<IFoo>>();
    Assert.IsNotNull(lazyFoo.Value);

## Function Factories ##

Function factories allows services to resolved as a function delegate that in turn is capable of returning the underlying service instance. We can think of this as an alternative to the [Service Locator](http://en.wikipedia.org/wiki/Service_locator_pattern) (anti)pattern.

    public interface IFoo {}
    public class Foo : IFoo {}

---

    container.Register<IFoo,Foo>();
    var func = container.GetInstance<Func<IFoo>>();
    var foo = func();
    Assert.IsNotNull(foo); 

>**Note:** *A function factory is effectively a delegate that redirects back to the corresponding **GetInstance** method on the service container.*

### Named Factories ###

The container returns a function delegate that represents calling the **GetInstance** method with "SomeFoo" as the service name argument.

    container.Register<IFoo, Foo>("SomeFoo");
    var func = container.GetInstance<Func<IFoo>>("SomeFoo");   
    var foo = func();
    Assert.IsNotNull(foo);


### Parameters ###

Function factories can also take parameters that will be used create the service instance.

    public class Foo : IFoo
    {
        public Foo(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }

---

    container.Register<int, IFoo>((factory, value) => new Foo(value));
    var fooFactory = container.GetInstance<Func<int, IFoo>>();
    var foo = (Foo)fooFactory(42); 
    Assert.AreEqual(foo.Value, 42);

>**Note** : *The service must be explicitly registered in order for the container to resolve it as a parameterized function factory.*

### IDisposable ###

The only way to deal with disposable objects when using function factories, is to let the service type inherit from IDisposable.
 
    public interface IFoo : IDisposable {}
    public class Foo : IFoo {}

---

    container.Register<IFoo, Foo>();
    var fooFactory = container.GetInstance<Func<IFoo>>();

    using(IFoo foo = fooFactory())
    {
        
    } <--Instance is disposed here          

>**Note:** *Although this is common practice even in the [BCL](http://en.wikipedia.org/wiki/Base_Class_Library), this kind of interfaces are often referred to as [leaky abstractions](http://en.wikipedia.org/wiki/Leaky_abstraction).*

## Typed Factories  ##

A typed factory is a class that wraps the function factory that is used to create the underlying service instance.
As opposed to just function factories, typed factories provides better expressiveness to the consumer of the factory.   
    
    public interface IFooFactory
    {
        IFoo GetFoo();
    }

---

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

---
    
    container.Register<IFoo, Foo>();
    container.Register<IFooFactory, FooFactory>(new PerContainerLifetime());
    var fooFactory = container.GetInstance<IFooFactory>();
    var foo = fooFactory.GetFoo();
    Assert.IsNotNull(foo);

>**Note:** *Register typed factories with the **PerContainerLifetime** unless a compelling reason exists to choose a different lifetime.*  

### Parameters ###

Types factories can also wrap a parameterized function factory and allows us to pass arguments.

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

---

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

---

    container.RegisterFactory<int, IFoo>((factory, value) => new Foo(value));
    container.Register<IFooFactory, FooFactory>(new PerContainerLifetime());
    var typedFooFactory = container.GetInstance<IFooFactory>();
    var foo = typedFooFactory.GetFoo(42);
    Assert.AreEqual(foo.Value, 42);

### IDisposable ###

Working with typed factories gives us the possibility to release disposable services registered as transients without exposing a leaky abstraction.

    public interface IFooFactory
    {
        IFoo GetFoo(int value);
        void Release(IFoo foo);
    } 

---

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

## Recursive dependency detection ##

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

## Internals ##

When running under the .Net platform, **LightInject** is capable of creating instances of classes that has the [internal](http://msdn.microsoft.com/en-us/library/7c5ka91b(v=vs.110).aspx) modifier. 

The only requirement is that the internal class exposes a public constructor.

    internal class InternalFooWithPublicConstructor : IFoo
    {
        public InternalFooWithPublicConstructor () {}
    }