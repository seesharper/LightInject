# Annotation  #

**LightInject** supports annotation of properties and constructor parameters through an extension LightInject. 

<div class="nuget-badge" >
   <p>
         <code>PM&gt; Install-Package LightInject.Annotation </code>
   </p>
</div> 

By using the **InjectAttribute** we can be more explicit about the services that gets injected into properties and constructor dependencies.

**Note:** *As opposed to all other types within **LightInject**, the **InjectAttribute** is marked with the public access modifier so that is can be used outside the assembly that contains the service container. This creates a dependency from our services to the assembly containing the attribute, but we still don't need to reference any of the specific types in **LightInject** making this an affordable sacrifice when it comes to services referencing container specific types.*  


### Property Injection ###

To enable annotated property injection, we must execute the following line before we start requesting services from the container.

	container.EnableAnnotatedPropertyInjection();


The container now only try to inject dependencies for properties that is annotated with the **InjectAttribute**.
The container will throw an **InvalidOperationException** if the annotated property dependency is unable to be resolved.

    public class FooWithAnnotatedProperyDependency : IFoo
    {
        [Inject]
        public IBar Bar { get; set; }
    }

Given that we have a registration for the **IBar** dependency, it will be injected into the **Bar** property.

   	container.Register<IFoo, FooWithAnnotatedProperyDependency>();
   	container.Register<IBar, Bar>();
    var instance = (FooWithAnnotatedProperyDependency)container.GetInstance<IFoo>();
    Assert.IsNotNull(instance.Bar);

If we have multiple registrations of the same interface, we can also use the **InjectAttribute** to specify the service to be injected. 

    public class FooWithNamedAnnotatedProperyDependency : IFoo
    {
        [Inject("AnotherBar")]
        public IBar Bar { get; set; }
    }

The container will inject the service that matches the specified service name.

	container.Register<IFoo, FooWithNamedAnnotatedProperyDependency>();
	container.Register<IBar, Bar>("SomeBar");
	container.Register<IBar, AnotherBar>("AnotherBar");	
	var instance = (FooWithNamedAnnotatedProperyDependency)container.GetInstance<IFoo>();	
	Assert.IsInstanceOfType(instance.Bar, typeof(AnotherBar));

### Constructor Injection ###

To enable annotated constructor injection, we must execute the following line before we start requesting services from the container.

	container.EnableAnnotatedConstructorInjection();

**LightInject** does consider all constructor parameters to be required dependencies and will try to satisfy all dependencies regardless of being annotated with the **InjectAttribute**. We can however use the **InjectAttribute ** to specify the named service to be injected.

    public class FooWithNamedAnnotatedDependency : IFoo
    {
        public FooWithNamedAnnotatedDependency([Inject(ServiceName="AnotherBar")]IBar bar)
        {
            Bar = bar;
        }

        public IBar Bar { get; private set; }
    }

The container will inject the service that matches the specified service name.

	container.Register<IFoo, FooWithNamedAnnotatedDependency>();
	container.Register<IBar, Bar>("SomeBar");
	container.Register<IBar, AnotherBar>("AnotherBar");	
	var instance = (FooWithNamedAnnotatedDependency)container.GetInstance<IFoo>();	
	Assert.IsInstanceOfType(instance.Bar, typeof(AnotherBar));
