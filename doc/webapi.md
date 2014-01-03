# Web API #

**LightInject.WebApi** provides an integration that enables dependency injection in ASP.NET Web API applications.

## Installing ##

**LightInject.WebApi** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.WebApi </code>
   </p>
</div>

This adds a reference to the **LightInject.WebApi.dll** in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.WebApi.Source </code>
   </p>
</div>

This will install a single file, **LightInject.WebApi.cs** in the target project.

## Initializing ##

    protected void Application_Start()
    {
        var container = new ServiceContainer();
        container.RegisterApiControllers();        
        //register other services
        
        container.EnableWebApi()              
    }

## FilterAttribute ##

Although filter attributes are instantiated by the MFC infrastructure, **LightInject** is still able to inject dependencies into properties.

    public class FooFilterAttribute : ActionFilterAttribute
    {
        public IFoo Foo { get; set; }
    }



 