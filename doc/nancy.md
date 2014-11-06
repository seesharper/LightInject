# Nancy #

**LightInject.Nancy** provides an integration that enables **LightInject** to be used as the IoC container in the Nancy web framework.

## Installing ##

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Nancy </code>
   </p>
</div>

This adds a reference to the **LightInject.Nancy.dll** in the target project.


## Dependencies ##



	public interface IFoo {}
	public class Foo : IFoo {}

	public class SampleModule : NancyModule
	{
		public SampleModule(IFoo foo)
		{
			Get["/"] = parameters => "Hello World";
		}
	}


Configuring additional services/dependencies is done by implementing the **ICompositionRoot** interface.

    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IFoo, Foo>();
        }
    }


> *Note: **LightInject** will look for an **ICompositionRoot** implementation in the same assembly as the requested service.*

## Custom Bootstrapper ##

If we need to use an existing container instance or perform some other configuration, this can be done by inheriting from the **LightInjectNancyBootstrapper** class. 

    public class Bootstrapper : LightInjectNancyBootstrapper
    {
        protected override IServiceContainer GetServiceContainer()
        {
            // Alteratively provide an existing container instance.
            return base.GetServiceContainer();
        }
    }

## Lifetime ##

Services registered with the **PerScopeLifetime** are scoped per web request while services registered with the **PerContainerLifetime** are as scoped per application. 

