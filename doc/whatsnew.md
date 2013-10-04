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

 
