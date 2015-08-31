# Interception #

**LightInject** supports [Aspect Oriented Programming](http://en.wikipedia.org/wiki/Aspect-oriented_programming) through proxy-based method interceptors. 

## Installing ##

**LightInject.Interception** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Interception </code>
   </p>
</div>

This adds a reference to the LightInject.Interception.dll in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Interception.Source </code>
   </p>
</div>

This will install a single file (LightInject.Interception.cs) into the current project.

 
## Interceptors ##

An interceptor sits between the call site and the target instance and intercepts method calls.

```
public class SampleInterceptor : IInterceptor
{
    public object Invoke(IInvocationInfo invocationInfo)
    {
        // Perform logic before invoking the target method
        var returnValue = invocationInfo.Proceed();
        // Perform logic after invoking the target method
        return returnValue;           
    }        
}
```

The **IInvocationInfo** instance passed into the **Invoke** method contains information about the method being intercepted.

   
 

The **Proceed** method calls down the chain of interceptors and ultimately the actual target instance.   

## Single Interceptor ##

This example shows how to configure the service container with a single interceptor to handle all method calls.

   
    container.Register<IFoo, Foo>();
    container.Intercept(sr => sr.ServiceType == typeof(IFoo), sf => new SampleInterceptor());

    var instance = container.GetInstance<IFoo>();

The instance returned is a proxy object that forwards method calls to the **SampleInterceptor** class.

The first parameter of the **Intercept** method is a selector function used to select the services that should have this interceptor applied.         
The second parameter is a function delegate that used to create an **IInterceptor** instance. 

**Note:** *Proxy types are lazy in the sense that they will not create the target instance or any interceptors until the first method call is made.* 


## Dependencies ##

Interceptors might also have dependencies and by resolving the interceptor through the container, those dependencies can be injected into the interceptor itself. 

   
    public class SampleInterceptor : IInterceptor
    {
        private IBar bar;

        public SampleInterceptor(IBar bar) 
        {
            this.bar = bar;    
        }

        public object Invoke(IInvocationInfo invocationInfo)
    
            // Perform logic using the injected dependency before invoking the target method             
            return invovationInfo.Proceed();                      
            // Perform logic using the injected dependency after invoking the target method
        }        
    }

The following example shows how to configure the container so that the **SampleInterceptor** instance is resolved through the container.

    container.Register<IFoo, Foo>()
    container.Register<IBar, Bar>();
    container.Register<IInterceptor, SampleInterceptor>();
    container.Intercept(sr => sr.ServiceType == typeof(IFoo), sf => sf.GetInstance<IInterceptor>()); 

**Note:** *When injecting depndencies into an interceptor we must make sure that the injected dependency is NOT intercepted by the same interceptor as this would cause a **StackOverFlowException**.*  



## Multiple Interceptors ##

Interceptors can be set up to handle a lot of cross cutting concerns such as logging, caching, null check and so on.
According to the [Single Responsibility Principle](http://en.wikipedia.org/wiki/Single_responsibility_principle), we can separate the combined logic into a set of interceptor that each only does "one" thing.

We can do this by using another overload of the **Intercept** method that allows us to set up a **ProxyDefinition** instance that gives us more control over the proxy type that is being created.

 

    container.Intercept(sr => sr.ServiceType == typeof(IFoo), (sf,pd) =>  DefineProxyType(pd));

    private void DefineProxyType(ProxyDefinition proxyDefinition)
    {
        proxyDefinition.Implement(new FirstInterceptor());
        proxyDefinition.Implement(new SecondInterceptor());
    }

**Note:** *The interceptors are invoked in the same order as they are registered with the **Implement** method.*

## Method Selectors ##

Method selectors are used to select the methods that should be intercepted by an interceptor.
 
The following example shows how to set up the container so that only calls method **A** is passed to the interceptor.

    container.Intercept(sr => sr.ServiceType == typeof(IFoo), (sf, pd) =>  DefineProxyType(pd));

    private void DefineProxyType(ProxyDefinition proxyDefinition)
    {
        proxyDefinition.Implement(() => new SampleInterceptor(), m => m.Name == "SomeMethodName");       
    }

Methods that does not match the method selector predicate will NOT be intercepted and method calls will be passed directly down to the target instance.  

If we omit the method selector, **LightInject** will intercept all methods from the target type and any additional interface, except methods that are inherited from [System.Object](http://msdn.microsoft.com/en-us/library/system.object.aspx).    

* Equals(Object)
* GetHashCode
* GetType
* ToString

If we choose to use a method selector, these methods will also be intercepted if they match the predicate in the method selector.

    proxyDefinition.Implement(() => new SampleInterceptor(), m => m.IsDeclaredBy<object>());

We can also use a method selector with the **Intercept** method that allows easy interception of any method without implementing an **IInterceptor**.

	container.Intercept(m => m.Name == "SomeMethodName", invocationInfo => invocationInfo.Proceed());

 


### Extension Methods ###

LightInject provides a set of extension method that simplifies method selector predicates.

* IsPropertySetter - Returns **true** if the method represents a property setter, otherwise **false**. 
* IsPropertyGetter - Returns **true** if the method represents a property getter, otherwise **false**.
* GetProperty - Returns the property for which the target method either represents the property getter or the property setter.
 


## Chaining Interceptors ##

As already seen in the example with multiple interceptors,  we can chain interceptors together. We can also combine this with method selectors that will affect the call sequence from the call site down to the actual target instance.

Consider an interface with three methods.

    public interface IFoo 
    {
        void A();
        void B();
        void C();
    }

The following example shows how we can control the call sequence for each method.

    container.Intercept(sr => sr.ServiceType == typeof(IFoo), (sf, pd) =>  DefineProxyType(pd));

    private void DefineProxyType(ProxyDefinition proxyDefinition)
    {
        proxyDefinition.Implement(() => new FirstInterceptor(), m => m.Name == "A");
        proxyDefinition.Implement(() => new SecondInterceptor(), m => m.Name == "B");   
        proxyDefinition.Implement(() => new ThirdInterceptor(), m => m.Name == "A" || m.Name == "B" || m.Name == "C");
    }

**Method A call sequence**

FirstInterceptor -> ThirdInterceptor -> Target 


**Method B call sequence**

SecondInterceptor -> ThirdInterceptor -> Target


**Method C call sequence**

ThirdInterceptor -> Target

## Implementing additional interfaces ##

Another powerful feature of proxy objects is the ability to implement additional interfaces that is not implemented by the target type.

The **Intercept** method has an overload that lets us specify a set of interfaces to be implemented by the proxy type.

    container.Intercept(sr => sr.ServiceType == typeof(IFoo), new []{ typeof(IBar) }, (sf, pd) =>  DefineProxyType(pd));
 
    private void DefineProxyType(ProxyDefinition proxyDefinition)
    {
        proxyDefinition.Implement(() => new BarInterceptor(), m => m.IsDeclaredBy<IBar>());        
    } 


When implementing additional interfaces we must make sure that all methods are intercepted by either one or a combined set of interceptors. This is because we are now dealing with methods that does not exist in the target type and we must do all implementation through interceptors.  

## IProxy ##

    /// <summary>
    /// Implemented by all proxy types.
    /// </summary>
    public interface IProxy
    {
        /// <summary>
        /// Gets the proxy target.
        /// </summary>
        object Target { get; }
    }

We can get to the underlying target instance through the **IProxy** interface

    container.Register<IFoo, Foo>();
    container.Intercept(sr => sr.ServiceType == typeof(IFoo), sf => new SampleInterceptor());

    var instance = container.GetInstance<IFoo>();
    var actualTarget = ((IProxy)instance).Target;
      


## This ##

One of the things to be aware of when working with proxy based interception is that it all relies on method calls being made through the proxy.
Method calls that are made directly to the target instance will NOT be intercepted. 

    public interface IFoo
    {
        void A();
    }

    public class Foo : IFoo
    {
        public void A() {}

        private void B()
        {
            //Calls the target (this) directly and interceptors are not invoked.
            this.A();
        }
    }

Another scenario is when the proxy instance itself is leaking its target.

    public interface IFoo
    {
        IFoo A();
    }

    public class Foo
    {
        public IFoo A()
        {
            return this;
        }
    }

**LightInject** will take care of this scenario and detect that we are about to return **this** from a method and replace the return value with the proxy instance instead. 

Other scenarios such as event handlers or passing "this" to another method is NOT taken care of by **LightInject** as it is not possible without modifying the code in the target type itself. 

## Class Proxies ##

Starting from version 1.0.0.4, **LightInject.Interception** can be used to intercept classes with virtual members.
	
	public class Foo
	{
		public virtual void A()
		{
		}
	}

Any member that is marked as virtual can be intercepted.	

	var container = new ServiceContainer();
    container.Register<Foo>();
    container.Intercept(sr => sr.ServiceType == typeof(Foo), factory => new SampleInterceptor());

Class proxies are implemented internally by subclassing the target type and overriding virtual members to support interception.  