# WCF #

**LightInject.Wcf** provides an integration that enables dependency injection in WCF applications. 

## Installing ##

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Wcf </code>
   </p>
</div>

This adds a reference to the **LightInject.Wcf.dll** in the target project.

## Getting started ##

**LightInject.Wcf** aims to provide a zero config approach to developing **WCF** services in addition to support both constructor and property injection for **WCF** service implementations. 

The easiest way to create a new WCF application is to start with a new empty web application and then install the **LightInject.Wcf** package.

Our first service might look something like this:

    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        int GetValue(int value);
    }

    public class Service : IService
    {
        public int GetValue(int value)
        {
            return value;
        }
    }

The only thing we need to do is to create an ICompositionRoot implementation that registers our services and potentially its dependencies.

    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IService, Service>();
        }
    }

That's it. No XML config, no .svc files, just press F5 to run the application.

Given that our service is defined in the **SampleWcfApplication** namespace, the service will be available at 

	http://localhost:xxxxx/SampleWcfApplication.IService.svc	


## Named Services ##

If we want to identify our service by something else than the full type name of the service interface, we need to register the service using a name. 

    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IService, Service>("MyService");
        }
    }
 
Now we need one additional class to bootstrap the container so that the service is registered with the correct name before the service is invoked.

	[assembly: System.Web.PreApplicationStartMethod(typeof(SampleWcfApplication.Startup), "Initialize")]
	namespace SampleWcfApplication
	{
	    using LightInject;
	    using LightInject.Wcf;
	
	    public class Startup
	    {
	        public static void Initialize()
	        {
	            var container = new ServiceContainer();
	            container.RegisterFrom<CompositionRoot>();
	            LightInjectServiceHostFactory.Container = container;
	        }
	    }
	}


## Behaviors ##

While it still is possible to configure endpoint and service behaviors using XML, **LightInject.Wcf** allows for **IEndpointBehavior** and **IServiceBehavior** implementations to be registered with the container so that they can be applied to the service.


    public class SampleServiceBehavior : IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {            
        }

        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {            
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {           
        }
    }

	public class SampleEndpointBehavior : IEndpointBehavior
    {
        public void Validate(ServiceEndpoint endpoint)
        {            
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }
    }

In order to configure these endpoints we can simply register them with the container.

    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IService, Service>();
            serviceRegistry.Register<IServiceBehavior, SampleServiceBehavior>();
            serviceRegistry.Register<IEndpointBehavior, SampleEndpointBehavior>();
        }
    }



