## LightInject 3.0.2.4 ##
(Mai 22, 2015)

Fixes [issue #191](https://github.com/seesharper/LightInject/issues/191)
Fixes [issue #190](https://github.com/seesharper/LightInject/issues/190)

## LightInject 3.0.2.4 ##
(February 7, 2015)

Fixes [issue #163](https://github.com/seesharper/LightInject/issues/163)


## LightInject 3.0.2.3 ##
(January 22, 2015)

Fixes [issue #158](https://github.com/seesharper/LightInject/issues/158)

## LightInject 3.0.2.2 ##
(January 19, 2015)

Fixes [issue #156](https://github.com/seesharper/LightInject/issues/156)

## LightInject.Web 1.0.0.7 ##
(December 20, 2014)

Fixes [issue #143](https://github.com/seesharper/LightInject/issues/143)


## LightInject 3.0.2.1 ##
(December 12, 2014)

Fixes [issue #142](https://github.com/seesharper/LightInject/issues/142)
Fixes [issue #139](https://github.com/seesharper/LightInject/issues/139)

## LightInject.Interception 1.0.0.7 ##
(November 24, 2014)

Fixes [issue #134](https://github.com/seesharper/LightInject/issues/134)


## LightInject.Interception 1.0.0.6 ##

Fixes [issue #100](https://github.com/seesharper/LightInject/issues/100)
Fixes [issue #96](https://github.com/seesharper/LightInject/issues/96)
Fixes [issue #101](https://github.com/seesharper/LightInject/issues/101)

## LightInject 3.0.2.0 ##
(October 15, 2014)

Fixes [issue #116](https://github.com/seesharper/LightInject/issues/116)
Fixes [issue #117](https://github.com/seesharper/LightInject/issues/117)
Fixes [issue #118](https://github.com/seesharper/LightInject/issues/118)
Fixes [issue #119](https://github.com/seesharper/LightInject/issues/119)
Fixes [issue #120](https://github.com/seesharper/LightInject/issues/120)
Fixes [issue #124](https://github.com/seesharper/LightInject/issues/124)

## LightInject.Web 1.0.0.5 ##
(October 2, 2014)

Fixes [issue #113](https://github.com/seesharper/LightInject/issues/113)

## LightInject 3.0.1.9 ##
(October 2, 2014)

Fixes [issue #115](https://github.com/seesharper/LightInject/issues/115)

## LightInject 3.0.1.8 ##

Fixes [issue #102](https://github.com/seesharper/LightInject/issues/102)

Fixes [issue #103](https://github.com/seesharper/LightInject/issues/103)


## LightInject.WebApi 1.0.0.3 ##
(September 10, 2014)

Fixes [issue #97](https://github.com/seesharper/LightInject/issues/97) 


## LightInject.xUnit 1.0.0.1 ##
(September 4, 2014)


Added support for integrating with xUnit.

[Learn more](http://www.lightinject.net/#xunit)

## LightInject.Interception 1.0.0.5 ##
(June 6, 2014)

ProxyDefinition.TargetType now has a public getter.


## LightInject.Mvc 1.0.0.4 ##
(June 6, 2014)

Removed dependency to specific version to System.Web.Mvc 

## LightInject.SignalR 1.0.0.1 ##
(June 6, 2014)

Added support for ASP.NET SignalR

[Learn more](http://www.lightinject.net/#signalr)


## LightInject.Interception 1.0.0.4 ##
(June 6, 2014)

Added support for class based proxies

[Learn more](http://www.lightinject.net/#interception)

## LightInject.Interception 1.0.0.3 ##
(Mars 17, 2014)

Fixes [issue #62](https://github.com/seesharper/LightInject/issues/62) 

## LightInject 3.0.1.6 ##
(Mars 17, 2014)

Added support for scoped instances across await points.


## LightInject 3.0.1.5 ##
(February 21, 2014)

Fixes [issue #61](https://github.com/seesharper/LightInject/issues/61)



## LightInject 3.0.1.4 ##
(February 16, 2014)

Fixed a bug that caused resolving open generic types to fail when the [Curiously recurring template pattern](http://en.wikipedia.org/wiki/Curiously_recurring_template_pattern) was implemented.	 


## LightInject.WebApi 1.0.0.1 ##
(February 07, 2014)

Adds support for dependency injection in a Web API application.

[Learn more](index.html#webapi)

## LightInject 3.0.1.3 ##
(February 07, 2014)

Performance improvements by replacing the dictionary used for service lookup with a balanced binary tree.

Fixes [https://github.com/seesharper/LightInject/issues/57](https://github.com/seesharper/LightInject/issues/57)

Fixes [https://github.com/seesharper/LightInject/issues/49](https://github.com/seesharper/LightInject/issues/49)

## LightInject.Web 1.0.0.3##
(February 07, 2014)

Fixes [https://github.com/seesharper/LightInject/issues/57](https://github.com/seesharper/LightInject/issues/57)

Fixes [https://github.com/seesharper/LightInject/issues/49](https://github.com/seesharper/LightInject/issues/49)

## LightInject 3.0.1.0##

(November 08, 2013)
### Unresolved services ###

The default behavior was to scan the assembly that contains the unresolved service and register all services found in that assembly. 

As this could cause services to unintentially be registered into the container, this behaviour has now been changed.

The container will now look for ICompositionRoot implementations in the containing assembly. If they are found, they will be created and executed.


### Composition Roots ###

Added the **RegisterFrom&lt;TCompositionRoot&gt;** method that enables explicit execution of a composition root.

	container.RegisterFrom<SampleCompositionRoot>();

### Instances ###

The register method that registers an object instance with the container is now called **RegisterInstance**.    


## LightInject.Mvc 1.0.0.1 ##
(November 08, 2013)

**LightInject.Mvc** provides an integration that enables dependency injection in ASP.NET MVC applications.

    protected void Application_Start()
    {
        var container = new ServiceContainer();
        container.RegisterControllers();        
        //register other services
        
        container.EnableMvc()              
    }


<a href="#" onclick = "$('#mvc').trigger('click');"> Learn more... </a>




## LightInject 3.0.0.9##
(November 05, 2013)

### Binary Distribution ###

**LightInject** now offers a binary distribution in addition to the current distribution model that simply install the source files into the target project. This means that users now have the option of either using the binary (assembly) version or the source version.

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject</code>
   </p>
</div>

Binary distribution is now the "default" model for **LightInject** and users that still want to use the source version needs to remove the existing **LightInject** package and install the source version instead.
  
<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Source</code>
   </p>
</div>

These changes applies to all the LightInject packages such as **LightInject.Annotation**, **LightInject.Web** and so on. 

### Platform support ###

Added Windows Phone 8 and SilverLight 

### Parameterized service resolution ###

LightInject now support passing arguments when resolving services.

    container.Register<int, IFoo>((arg, factory) => new Foo(arg));
    var foo = (Foo)container.GetInstance<int, IFoo>(42);
    Assert.AreEqual(42,foo.Value);

<a href="#" onclick = "$('#gettingstarted').trigger('click');"> Learn more... </a>
  
## LightInject.Web 1.0.0.2 ##
(November 05, 2013)

Added a new extension method that more closely resembles the actual intent of **LightInject.Web**, which is to 
enable service to be scoped per web request. 

The following line of code 
  
    LightInjectHttpModule.SetServiceContainer(serviceContainer);

can now be replaced with

    container.EnabledPerWebRequestScope();

<a href="#" onclick = "$('#web').trigger('click');"> Learn more... </a>



## LightInject 3.0.0.8 ##

### Internals ###

When running under the .Net platform, **LightInject** is now capable of instantiating classes with the internal access modifier.

<a href="#" onclick = "$('#gettingstarted').trigger('click');"> Learn more... </a>


### Interception ###

LightInject now enables Aspect Oriented Programming through method interception. 

<a href="#" onclick = "$('#interception').trigger('click');"> Learn more... </a>


### Decorators - Bugfix ###

Decorators were not applied when the service was registered as a value.

    var foo = new Foo();    
    container.Register<IFoo>(foo);




## LightInject 3.0.0.7##

###Lazy&lt;T&gt;###

Services can now be resolved as lazy instances.

    container.Register<IFoo, Foo>()
    var lazyInstance = container.GetInstance<Lazy<IFoo>>();

<a href="#" onclick = "$('#gettingstarted').trigger('click');"> Learn more... </a>

###Mocking open generic types###

    container.Register(typeof(IFoo<>), typeof(Foo<>));
    container.StartMocking(typeof(IFoo<>), typeof(FooMock<>));

    var instance = container.GetInstance<IFoo<int>>();

    Assert.IsInstanceOfType(instance, typeof(FooMock<int>));

<a href="#" onclick = "$('#unittesting').trigger('click');"> Learn more... </a>

###Lazy Registration###

Lazy registration means that we can register services on a need to have basis. We are no longer restricted to just one composition root that needs to reference all assemblies that possibly contain services that should be configured into the container.

<a href="#" onclick = "$('#gettingstarted').trigger('click');"> Learn more... </a>

###Overall performance improvements###

Although **LightInject** already performs very well, a significant amount of work has been put into further performance improvements.



 


## LightInject 3.0.0.6##

### WinRT ##

LightInject now implements full support for WinRT making it the perfect choice for Windows Store Apps. We can now leverage the same DI framework all across the layers even if some parts are based on .Net and others on WinRT.

### Assembly scanning ###
 
When scanning an assembly LightInject provides a predicate that is used to filter services going into the service container.

Given this type in the target assembly.

    public class Foo : IFoo, IDisposeable
    { 
        ....
    }    

When scanning the containing assembly, this would cause to services to be registered. 
One that maps **IFoo** to **Foo** and another that maps **IDisposable** to **Foo**. 


    container.RegisterAssembly(someAssembly, implementingType => implementingType.Namespace == "SomeNamespace");


Starting from version 3.0.0.6 this predicate has changed to provide both the service type and the implementing type.

This means that we now have more control over services going into the service container during assembly scanning.

    container.RegisterAssembly(someAssembly, (servicetype,implementingType) => serviceType != typeof(IDisposable)
    && implementingType.Namespace == "SomeNamespace");

 
