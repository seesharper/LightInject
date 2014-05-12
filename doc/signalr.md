# SignalR #

**LightInject.Signal** provides an integration that enables dependency injection in [SignalR](http://signalr.net/) hub implementations.

## Installing ##

**LightInject.SignalR** provides two distribution models via NuGet

### Binary ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.SignalR </code>
   </p>
</div>

This adds a reference to the **LightInject.SignalR.dll** in the target project.

### Source ###

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.SignalR.Source </code>
   </p>
</div>

This will install a single file (**LightInject.SignalR.cs**) into the current project. 

## Initializing ##

The following example shows how to enable support for [SignalR](http://signalr.net/) in an [OWIN](http://owin.org/) startup class.

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var serviceContainer = new ServiceContainer();
			serviceContainer.RegisterHubs();            
		 	..register other services
			                                                                                                
            app.MapSignalR(serviceContainer.EnableSignalR());
        }
    }

## Services ##

Services that implements [IDisposable](http://msdn.microsoft.com/en-us/library/system.idisposable.aspx) must be registered with the with the **PerScopeLifetime** or the **PerRequestLifetime** to ensure that they are properly disposed when the [Hub](http://msdn.microsoft.com/en-us/library/microsoft.aspnet.signalr.hub(v=vs.118).aspx) is disposed.  







