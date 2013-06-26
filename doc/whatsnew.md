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

 
