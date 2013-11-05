#Unit testing#

**LightInject** provides native support for mocking services during unit testing.

## Installing ##

**LightInject.Mocking** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Mocking </code>
   </p>
</div>

This adds a reference to the LightInject.Mocking.dll in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Mocking.Source </code>
   </p>
</div>

This will install a single file (LightInject.Mocking.cs) into the current project.

**Note:** *Use the [InternalVisibleTo](http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.internalsvisibletoattribute.aspx) attribute to give the test project access to the **LightInject** internal types.*

## Basic types ##

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





##Open Generic Types##

    public class Foo<T> : IFoo<T>
    {
    }

Open generic types can be mocked by using type based mocking.

    container.Register(typeof(IFoo<>), typeof(Foo<>));
    container.StartMocking(typeof(IFoo<>), typeof(FooMock<>));

    var instance = container.GetInstance<IFoo<int>>();

    Assert.IsInstanceOfType(instance, typeof(FooMock<int>));