# LightInject.Xunit #

**LightInject.Xunit** provides an integration that enables dependency injection in [Xunit](https://github.com/xunit/xunit) test methods.

## Installing ##

**LightInject.Xunit** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Xunit </code>
   </p>
</div>

This adds a reference to the **LightInject.Xunit.dll** in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Xunit.Source </code>
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
 
## Configuration ##

**LightInject** will look for an **ICompositionRoot** implementation in the same assembly as the requested service. It it is found, it will be executed and the container gets configured through that composition root.   

If such an implementation does not exists or that we for some other reason need to configure the container, we can do this by simply implementing a static method in the test class with the following signature.

	public static void Configure(IServiceContainer container)
	{
	    container.Register<IFoo, Foo>();            
	}

This method is executed regardless of other composition roots and allows customized configuration of the container before the test is executed.  

## Scoping ##

**LightInject.Xunit** will start a new **Scope** when the tests starts so that services registered with the **PerScopeLifetime** or **PerRequestLifetime** are properly disposed when the test method ends. 



 