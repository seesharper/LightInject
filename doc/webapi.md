# Web API #

**LightInject.WebApi** provides an integration that enables dependency injection in Web API applications.

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
        container.EnablePerWebRequestScope();
        container.EnableWebApi(GlobalConfiguration.Configuration)              
    }

>**Note:** EnablePerWebRequestScope is only required for hosting within ASP.Net 


## FilterAttribute ##

Although filter attributes are instantiated by the MFC infrastructure, **LightInject** is still able to inject dependencies into properties.

    public class FooFilterAttribute : ActionFilterAttribute
    {
        public IFoo Foo { get; set; }
    }


## Owin Selfhosting ##

This example shows how to do Web API self hosting using OWIN.

### Step 1 ###

Create a standard console application and run the following command from the package manager console.

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package Microsoft.AspNet.WebApi.OwinSelfHost </code>
   </p>
</div>



### Step 2 ###

Add a OWIN startup class.

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {                        
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();          
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            app.UseWebApi(config); 
        }
    }
 

### Step 3 ###

Add a controller

    public class ValuesController : ApiController
    {        
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }        
    } 

### Step 4 ###

Modify the *Main* method to start the OWIN host.

    class Program
    {
        static void Main(string[] args)
        {            
            // Start OWIN host 
            using (WebApp.Start<Startup>("http://localhost:9000/"))
            {
                Console.ReadLine(); 
            }

            Console.ReadLine(); 
        }
    }  

Press **F5** to run the application and browse to [http://localhost:9000/api/values](http://localhost:9000/api/values).

### Step 5 ###
  
<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.WebApi </code>
   </p>
</div>

Modify the *Startup* class to enable LightInject to be used as the dependency resolver.

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {                        
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            var container = new ServiceContainer();
            container.RegisterApiControllers();
            container.EnableWebApi(config);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            app.UseWebApi(config); 
        }
    }   

> Note: LightInject.Web can be uninstalled as it is not used in the self host scenario. 


### Scoping ###

Scopes are handled by Web API itself and services registered with the PerScopeLifetime or PerRequestLifetime are disposed when the web request ends.

### HttpRequestMessage ###

The current *HttpRequestMessage* is available to us in the controllers as it is exposed through the *Request* property.
To make the *HttpRequestMessage* available to other services we need to make some minor changes

> The following code will be included as part of the next version of LightInject.WebApi.

The first thing we need is a handler that can keep track of the current *HttpRequestMessage*.

    internal class HttpRequestMessageHandler : DelegatingHandler
    {
        private LogicalThreadStorage<HttpRequestMessageStorage> messageStorage =
            new LogicalThreadStorage<HttpRequestMessageStorage>(() => new HttpRequestMessageStorage());
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            messageStorage.Value.Message = request;            
            return base.SendAsync(request, cancellationToken);
        }

        public HttpRequestMessage GetCurrentMessage()
        {
            return messageStorage.Value.Message;
        }
    }

The *HttpRequestMessageStorage* class is simply a "holder" class for the actual *HttpRequestMessage*.

    public class HttpRequestMessageStorage
    {
        public HttpRequestMessage Message { get; set; }
    }

Next we modify the Startup class to support injection of a *Func&lt;HttpRequestMessage&gt;*.

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {                                                
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            var container = new ServiceContainer();
            container.RegisterApiControllers();
            container.EnableWebApi(config);
            container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            var handler = new HttpRequestMessageHandler();
            config.MessageHandlers.Insert(0, handler);
            container.Register<Func<HttpRequestMessage>>(factory => () => handler.GetCurrentMessage());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            app.UseWebApi(config); 
        }
    }

Now, if we need access to the current *HttpRequestMessage*, we can just inject a function delegate.

	public class Foo
	{
		public Foo(Func<HttpRequestMessage> getCurrentRequestMessage)
		{
			var currentMessage = getCurrentRequestMessage();
		}

	}	
