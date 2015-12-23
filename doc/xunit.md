# LightInject.xUnit #

**LightInject.xUnit** provides an integration that enables dependency injection in [xUnit](https://github.com/xunit/xunit) test methods.

## Installing ##

**LightInject.xUnit** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.xUnit </code>
   </p>
</div>

This adds a reference to the **LightInject.Xunit.dll** in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.xUnit.Source </code>
   </p>
</div>

This will install a single file, **LightInject.Xunit.cs** in the target project.

## Injecting services ##

Services from **LightInject** are injected into methods that are decorated with the **InjectData** attribute. 


	[Theory, InjectData]                
	public void TestMethod(IFoo foo)
	{
	    Assert.NotNull(foo);
	}
 

## Injecting services and data 

Starting with **LightInject.xUnit.2.0.01** we can do a combination of injecting services instances and data.

```
[Theory]
[InjectData(0, 0, 0)]
[InjectData(1, 1, 2)]
[InjectData(2, 2, 4)]
[InjectData(5, 5, 10)]
public void ShouldAddNumbers(ICalculator calculator, int first, int second, int expected)
{
	int result = calculator.Add(first, second);		
	Assert.Equal(expected, result)
}
```

## Configuration ##

**LightInject** will look for an **ICompositionRoot** implementation in the same assembly as the requested service. If it is found, it will be executed and the container gets configured through that composition root.   

If such an implementation does not exists or that we for some other reason need to configure the container, we can do this by simply implementing a static method in the test class with the following signature.

	public static void Configure(IServiceContainer container)
	{
	    container.Register<IFoo, Foo>();            
	}

This method is executed regardless of other composition roots and allows customized configuration of the container before the test is executed.  
 

## Scoping (xUnit <= 1.9.2)##

Services registered with the **PerScopeLifetime** or **PerRequestLifetime** needs to be resolved within an active **Scope** to ensure that any services that implements **IDisposable** are properly disposed.  

By decorating the test method with the **ScopedTheory** attribute, a new **Scope** will be started when the test method starts and it will end when the test method ends.   
	
	[ScopedTheory, InjectData]
	public void MethodWithScopedArgument(IFoo foo)
	{
	    Assert.NotNull(foo);
	}

## Scoping (xUnit >= 2.0.0)##

Services registered with the **PerScopeLifetime** or **PerRequestLifetime** needs to be resolved within an active **Scope** to ensure that any services that implements **IDisposable** are properly disposed.  

By decorating the test method with the **Scoped** attribute, a new **Scope** will be started when the test method starts and it will end when the test method ends.   
	
	[Theory, Scoped, InjectData]
	public void MethodWithScopedArgument(IFoo foo)
	{
	    Assert.NotNull(foo);
	}


 