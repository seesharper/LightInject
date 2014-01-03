# MVC #

**LightInject.Mvc** provides an integration that enables dependency injection in ASP.NET MVC applications. 

## Installing ##

**LightInject.Mvc** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Mvc </code>
   </p>
</div>

This adds a reference to the LightInject.Mvc.dll in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Mvc.Source </code>
   </p>
</div>

This will install a single file (LightInject.Mvc.cs) into the current project.


## Initializing ##

    protected void Application_Start()
    {
        var container = new ServiceContainer();
        container.RegisterControllers();        
        //register other services
        
        container.EnableMvc()              
    }

   
    

## Services ##

All services that implements IDisposable,  must be registered with the **PerScopeLifetime** to ensure that they are properly disposed when the web request ends.

    container.Register<IFoo, Foo>(new PerScopeLifetime());

Controllers are also disposable services and **LightInject** provides the **RegisterControllers** method that registers all controllers from a given assembly with the **PerRequestLifetime**. 

    container.RegisterControllers(typeof(MyMvcApplication).Assembly);

 


## FilterAttribute ##

Although filter attributes are instantiated by the MFC infrastructure, **LightInject** is still able to inject dependencies into properties.

    public class FooFilterAttribute : ActionFilterAttribute
    {
        public IFoo Foo { get; set; }
    }        