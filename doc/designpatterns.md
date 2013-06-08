# Design Patterns #

LightInject has built-in support for the Decorator pattern and the Composite pattern.

## Decorator Pattern ##

A decorator is a class that implements the same interface as the type it is decorating and takes the target instance as a constructor argument.

	public class FooDecorator : IFoo
	{     
	    public FooDecorator(IFoo foo)
	    {     
	    }
	} 
	

Decorators are applied using the **Decorate** method.	
    
	container.Register<IFoo, Foo>();
    container.Decorate(typeof(IFoo), typeof(FooDecorator));

    var instance = container.GetInstance<IFoo>();

    Assert.IsInstanceOfType(instance, typeof(FooDecorator));

Decorators can be nested and they are applied in the same sequence as they are registered.

    container.Register<IFoo, Foo>();            
    container.Decorate(typeof(IFoo), typeof(FooDecorator));
    container.Decorate(typeof(IFoo), typeof(AnotherFooDecorator));

    var instance = container.GetInstance<IFoo>();

    Assert.IsInstanceOfType(instance, typeof(AnotherFooDecorator));

If we have multiple services implementing the same interface, we can apply the decorator to implementations matching the given predicate.
    
    container.Register<IFoo, Foo>();
    container.Register<IFoo, AnotherFoo>("AnotherFoo");
    container.Decorate(typeof(IFoo), typeof(FooDecorator), service => service.ServiceName == "AnotherFoo");

    var instance = container.GetInstance<IFoo>();
    var decoratedInstance = container.GetInstance<IFoo>("AnotherFoo");

    Assert.IsInstanceOfType(instance, typeof(Foo));
    Assert.IsInstanceOfType(decoratedInstance, typeof(FooDecorator));

Decorators can have their own dependencies in addition to the target instance it is decorating.

    public class FooDecoratorWithDependency : IFoo
    {        
        public FooDecoratorWithDependency(IFoo foo, IBar bar)
        {
            Foo = foo;
            Bar = bar;
        }

        public IFoo Foo { get; private set; }

        public IBar Bar { get; private set; }
    }

The dependencies of the decorator can be implicitly resolved.

	container.Register<IFoo, Foo>();
	container.Register<IBar, Bar>();
	container.Decorate(typeof(IFoo), typeof(FooDecoratorWithDependency));
	var instance = (FooDecoratorWithDependency)container.GetInstance<IFoo>();
	Assert.IsInstanceOfType(instance.Foo, typeof(IFoo));
	Assert.IsInstanceOfType(instance.Bar, typeof(IBar));


By using a function factory, we can explicitly specify the depenendecies of the decorator.

    container.Register<IFoo, Foo>();
    container.Register<IBar, Bar>();
    container.Decorate<IFoo>((serviceFactory, target) 
        => new FooDecoratorWithDependency(target, serviceFactory.GetInstance<IBar>()));
    var instance = (FooDecoratorWithDependency)container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance.Foo, typeof(IFoo));
    Assert.IsInstanceOfType(instance.Bar, typeof(IBar));

**Note:** *The target instance is available through the function delegate so that we can pass it to the constructor of the decorator.*


Decorators can also be applied to open generic types.

    container.Register(typeof(IFoo<>), typeof(Foo<>));
    container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>));

    var instance = container.GetInstance<IFoo<int>>();

    Assert.IsInstanceOfType(instance, typeof(FooDecorator<int>));


## Composite Pattern ##

The [composite pattern](http://en.wikipedia.org/wiki/Composite_pattern) is a simple pattern that lets a class implement an interface and then delegates invocation of methods to a set other classes implementing the same interface. 



    public class FooWithEnumerableIFooDependency : IFoo
    {
        public IEnumerable<IFoo> FooList { get; private set; }

        public FooWithEnumerableIFooDependency(IEnumerable<IFoo> fooList)
        {
            FooList = fooList;
        }
    }

While this looks like a recursive dependency, **LightInject** detects this and removes the  **FooWithEnumerableIFooDependency** from the IEnumerable&lt;IFoo&gt; beeing injected.	 

    container.Register(typeof(IFoo), typeof(Foo), "Foo");
    container.Register(typeof(IFoo), typeof(AnotherFoo), "AnotherFoo");
    container.Register(typeof(IFoo), typeof(FooWithEnumerableIFooDependency));            
    var instance = (FooWithEnumerableIFooDependency)container.GetInstance<IFoo>();
    Assert.IsInstanceOfType(instance.FooList.First(), typeof(Foo));
    Assert.IsInstanceOfType(instance.FooList.Last(), typeof(AnotherFoo));