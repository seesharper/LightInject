# Nancy #

**LightInject.Nancy** provides an integration that enables **LightInject** to be used as the IoC container in the Nancy web framework.

## Installing ##

**LightInject.Nancy** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Nancy </code>
   </p>
</div>

This adds a reference to the **LightInject.Nancy.dll** in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Nancy.Source </code>
   </p>
</div>

This will install a single file, **LightInject.Nancy.cs** in the target project.

## Bootstrapper ##


    public class Bootstrapper : LightInjectNancyBootstrapper
    {
        protected override IServiceContainer GetServiceContainer()
        {
            // Alteratively provide an existing container instance.
            return base.GetServiceContainer();
        }

        protected override void Configure(IServiceContainer serviceContainer)
        {
            // Configure additonal services.
            base.Configure(serviceRegistry);
        }
    }

