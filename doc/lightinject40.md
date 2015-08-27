## LightInject 4.0 ##
LightInject 4.0 marks a new era for LightInject with support for a broader range of platforms including iOS, Android and the new CoreCLR.

With C# and .Net now being available on practically any platform and any device it becomes more and more important to provide cross-platform libraries.

Whether you are developing mobile applications or working on high performance server side applications,  LightInject provides the same simple API across all environments.

A lot of effort has been made into making sure that LightInject performs optimally regardless of the target platform and is cross-compiled into platform specific binaries.

In addition to a totally revamped build system (scriptcs), the unit  tests has also been migrated from MsTest to xUnit 2.

LightInject now supports the following frameworks.

* Net 4.0
* Net 4.5
* Net 4.6
* DNX451
* DNXCORE50
* Portable class library (Profile 111) 

## Limitations on iOS and Android
When working with open generic types on iOS and Android there is no support for enforcing generic constraints.

```C#
public class GenericFoo<T> : where T : IBar
{
	
}
```

Register the open generic type

```
container.Register(typeof(GenericFoo<>));
```

Even though the GenericFoo class declares a generic type constraint (IBar), the container will still be able to produce an instance using a type parameter that is not compatible with the type constraint.
```c#
var genericFoo = container.GetInstance<GenericFoo<string>>();
```

The reason for this is that generic type constraints are enforced by the JIT (Just In Time) compiler and when running on iOS and Android there is no JIT as everything is compiled using the AOT (Ahead Of Time) model.



