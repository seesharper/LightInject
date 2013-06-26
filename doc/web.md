# Web #

Enables **LightInject** to be used in a web application and provides support for **PerWebRequest** scoped service instances.

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Web </code>
   </p>
</div>

The following example shows how to enable **LightInject** in the **Application_Start** event.

	protected void Application_Start()
    {
		var serviceContainer = new ServiceContainer();            
		serviceContainer.Register<IFoo, Foo>(new PerScopeLifetime()); 
		LightInjectHttpModule.SetServiceContainer(serviceContainer);		
	}

A service	 registered with **PerScopeLifetime** is scoped per web request and is disposed at the end of the request if it implements **IDisposable**.