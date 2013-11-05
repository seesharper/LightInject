# Common Service Locator #

**LightInject** provides an implementation of the [Common Service Locator](http://commonservicelocator.codeplex.com/) adapter for seamless integration with frameworks that relies on this layer of abstraction.

## Installing ##

**LightInject.ServiceLocation** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.ServiceLocation </code>
   </p>
</div>

This adds a reference to the LightInject.ServiceLocation.dll in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.ServiceLocation.Source </code>
   </p>
</div>

This will install a single file (LightInject.ServiceLocation.cs) into the current project.

## Example ##

    var container = new ServiceContainer();
    container.Register<IFoo, Foo>();
    var serviceLocator = new LightInjectServiceLocator(container);

    var foo = serviceLocator.GetInstance<IFoo>();

    Assert.IsInstanceOfType(typeof(IFoo), foo);

