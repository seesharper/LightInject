# Fixie #

**LightInject.Fixie** provides an integration that enables **LightInject** to be used as the IoC container in the [Fixie](http://fixie.github.io/) unit test framework  

## Installing ##

**LightInject.Fixie** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Fixie </code>
   </p>
</div>

This adds a reference to the LightInject.Fixie.dll in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Fixie.Source </code>
   </p>
</div>

This will install a single file (LightInject.Fixie.cs) into the current project.

## Configuration ##

	public class MyTestAssembly : TestAssembly
	{
		public MyTestAssembly()
		{
			Apply<LightInjectConvention>();
		}
	}

> Note: Place the "MyTestAssembly" file in the same assembly as the unit tests.

By default, **LightInject.Fixie** will look for an **ICompositionRoot** implementation in the same assembly as the unit tests.

	public class TestCompositionRoot : ICompositionRoot
	{
		public void Compose(IServiceRegistry registry)
		{
			registry.Register<IFoo, Foo>();
		}
	}
  
If we need to register services that are specific to a test class, we can do this by simply implementing a static method in the test class with the following signature.

	public static void Configure(IServiceContainer container)
	{
	    container.Register<IFoo, Foo>();            
	}



## Constructor Injection ##

	public class ConstructorInjectionTests
	{
		public SampleTests(IFoo)
		{
			
		}
	} 

## Property Injection ##

	public class PropertyInjectionTests
	{
		public IFoo Foo { get; set; }
	}

## Method Injection ##

	public class MethodInjectionTests
	{
		public void SampleTest(IFoo foo)
		{
		}
	}

## Lifetime ##

**LightInject.Fixie** starts a new **Scope** each time a test class instance is created. The **Scope** ends when the tests within this instance are completed. A new **Scope** is also started for each test method and ends when the test method is completed.   