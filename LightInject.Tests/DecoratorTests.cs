namespace LightInject.Tests
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;

    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DecoratorTests : TestBase
    {
        [TestMethod]
        public void GetInstance_WithDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate(typeof(IFoo), typeof(FooDecorator));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_DecoratorWithDependency_ReturnsDecoratedInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>();
            container.Decorate(typeof(IFoo), typeof(FooDecoratorWithDependency));
            var instance = (FooDecoratorWithDependency)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.Foo, typeof(IFoo));
            Assert.IsInstanceOfType(instance.Bar, typeof(IBar));
        }

        [TestMethod]
        public void GetInstance_DecoratorWithDependencyFirst_ReturnsDecoratedInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>();
            container.Decorate(typeof(IFoo), typeof(FooDecoratorWithDependencyFirst));
            var instance = (FooDecoratorWithDependencyFirst)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.Foo, typeof(IFoo));
            Assert.IsInstanceOfType(instance.Bar, typeof(IBar));
        }


        [TestMethod]
        public void GetInstance_SingletonWithDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerContainerLifetime());
            container.Decorate(typeof(IFoo), typeof(FooDecorator));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_SingletonWithNestedDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerContainerLifetime());
            container.Decorate(typeof(IFoo), typeof(FooDecorator));
            container.Decorate(typeof(IFoo), typeof(AnotherFooDecorator));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFooDecorator));
        }

        [TestMethod]
        public void GetInstance_WithDecorator_DecoratesServicesAccordingToPredicate()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, AnotherFoo>("AnotherFoo");
            container.Decorate(typeof(IFoo), typeof(FooDecorator), service => service.ServiceName == "AnotherFoo");
            var instance = container.GetInstance<IFoo>();
            var decoratedInstance = container.GetInstance<IFoo>("AnotherFoo");
            Assert.IsInstanceOfType(instance, typeof(Foo));
            Assert.IsInstanceOfType(decoratedInstance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_WithNestedDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();            
            container.Decorate(typeof(IFoo), typeof(FooDecorator));
            container.Decorate(typeof(IFoo), typeof(AnotherFooDecorator));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFooDecorator));
        }

        [TestMethod]
        public void GetAllInstances_WithDecorator_ReturnsDecoratedInstances()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, AnotherFoo>("AnotherFoo");
            container.Decorate(typeof(IFoo), typeof(FooDecorator));
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsInstanceOfType(instances.First(), typeof(FooDecorator));
            Assert.IsInstanceOfType(instances.Last(), typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_WithOpenGenericDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>));
            var instance = container.GetInstance<IFoo<int>>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator<int>));
        }

        [TestMethod]
        public void GetInstance_OpenGenericDecoratorFollowedByClosedGenericDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));           
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>));
            container.Decorate(typeof(IFoo<int>), typeof(ClosedGenericFooDecorator));
            var instance = container.GetInstance<IFoo<int>>();           
            Assert.IsInstanceOfType(instance, typeof(ClosedGenericFooDecorator));
        }

        [TestMethod]
        public void GetInstance_ClosedGenericDecoratorFollowedByOpenGenericDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Decorate(typeof(IFoo<int>), typeof(ClosedGenericFooDecorator));
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>));            
            var instance = container.GetInstance<IFoo<int>>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator<int>));
        }

        [TestMethod]
        public void GetInstance_WithNestedOpenGenericDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>));
            container.Decorate(typeof(IFoo<>), typeof(AnotherFooDecorator<>));
            var instance = container.GetInstance<IFoo<int>>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFooDecorator<int>));
        }

        [TestMethod]
        public void GetInstance_ClosedGenericServiceWithOpenGenericDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<int>), typeof(Foo<int>));
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>));
            var instance = container.GetInstance<IFoo<int>>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator<int>));
        }

        [TestMethod]
        public void GetAllInstances_WithOpenGenericDecorator_ReturnsDecoratedInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>));
            var instances = container.GetAllInstances<IFoo<int>>();
            Assert.IsInstanceOfType(instances.First(), typeof(FooDecorator<int>));
            Assert.IsInstanceOfType(instances.Last(), typeof(FooDecorator<int>));
        }

        [TestMethod]
        public void GetAllInstances_OpenAndClosedGenericServiceWithOpenGenericDecorator_ReturnsDecoratedInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<int>), typeof(Foo<int>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>));
            var instances = container.GetAllInstances<IFoo<int>>();
            Assert.IsInstanceOfType(instances.First(), typeof(FooDecorator<int>));
            Assert.IsInstanceOfType(instances.Last(), typeof(FooDecorator<int>));
        }

        [TestMethod]
        public void GetInstance_OpenGenericDecoratorAfterClosedGenericDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>)); 
            // Register twice to provoke an decorator index == 1
            container.Decorate<IFoo<int>>((factory, foo) => new FooDecorator<int>(foo));
            container.Decorate<IFoo<int>>((factory, foo) => new FooDecorator<int>(foo));
            container.Decorate(typeof(IFoo<>), typeof(AnotherFooDecorator<>));

            var instance = container.GetInstance<IFoo<int>>();

            Assert.IsInstanceOfType(instance, typeof(AnotherFooDecorator<int>));
        }

        [TestMethod]
        public void GetInstance_DecoratorFactory_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate<IFoo>((serviceFactory, target) => new FooDecorator(target));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_DecoratorFactoryWithDependency_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>();
            container.Decorate<IFoo>((serviceFactory, target) 
                => new FooDecoratorWithDependency(target, serviceFactory.GetInstance<IBar>()));
            var instance = (FooDecoratorWithDependency)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.Foo, typeof(IFoo));
            Assert.IsInstanceOfType(instance.Bar, typeof(IBar));
        }

        [TestMethod]
        public void GetInstance_DecoratorFactoryWithDependencyFirst_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>();
            container.Decorate<IFoo>((serviceFactory, target)
                => new FooDecoratorWithDependencyFirst(serviceFactory.GetInstance<IBar>(), target));
            var instance = (FooDecoratorWithDependencyFirst)container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance.Foo, typeof(IFoo));
            Assert.IsInstanceOfType(instance.Bar, typeof(IBar));
        }

        [TestMethod]
        public void GetInstance_DecoratorFactoryWithMethodCall_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate<IFoo>((serviceFactory, target) => GetFooDecorator(target));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_ServicePredicate_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.RegisterFallback((serviceType, serviceName) => serviceType == typeof(IFoo), request => new Foo());
            container.Decorate(typeof(IFoo), typeof(FooDecorator));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_DecoratorWithLazyTarget_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate<IFoo>((factory, foo) => new LazyFooDecorator(new Lazy<IFoo>(factory.GetInstance<IFoo>)));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(LazyFooDecorator));
        }

        [TestMethod]
        public void GetInstance_DecoratorWithLazyTarget_DoesNotCreateTarget()
        {
            Foo.Instances = 0;
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate(typeof(IFoo), typeof(LazyFooDecorator));
            container.GetInstance<IFoo>();
            Assert.AreEqual(0, Foo.Instances);
        }

        [TestMethod]
        public void GetInstance_DecoratorWithLazyTarget_CreatesTargetWhenValuePropertyIsAccessed()
        {
            Foo.Instances = 0;
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate(typeof(IFoo), typeof(LazyFooDecorator));
            var instance = (LazyFooDecorator)container.GetInstance<IFoo>();                     
            Assert.IsInstanceOfType(instance.Foo.Value, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_NestedLazyDecorators_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate(typeof(IFoo), typeof(LazyFooDecorator));
            container.Decorate(typeof(IFoo), typeof(AnotherLazyFooDecorator));

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(AnotherLazyFooDecorator));
        }



        [TestMethod]
        public void GetInstance_NonLazyDecoratorFollowedByLazyDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate(typeof(IFoo), typeof(FooDecorator));
            container.Decorate(typeof(IFoo), typeof(LazyFooDecorator));

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(LazyFooDecorator));
        }

        [TestMethod]
        public void GetInstance_LazyDecoratorFollowedByNonLazyDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();           
            container.Decorate(typeof(IFoo), typeof(LazyFooDecorator));
            container.Decorate(typeof(IFoo), typeof(FooDecorator));

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }



        [TestMethod]
        public void GetInstance_SingletonInjectecIntoTwoDifferentClasses_DoesNotReapplyDecorators()
        {
            BarDecorator.Instances = 0;
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerContainerLifetime());
            container.Decorate<IBar, BarDecorator>();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IFoo, AnotherFooWithDependency>("AnotherFooWithDependency");

            container.GetInstance<IFoo>();
            container.GetInstance<IFoo>("AnotherFooWithDependency");

            Assert.AreEqual(1, BarDecorator.Instances);
        }

        [TestMethod]
        public void GetAllInstances_SingletonInjectecIntoTwoDifferentClasses_DoesNotReapplyDecorators()
        {
            BarDecorator.Instances = 0;
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerContainerLifetime());
            container.Decorate<IBar, BarDecorator>();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IFoo, AnotherFooWithDependency>("AnotherFooWithDependency");

            container.GetAllInstances<IFoo>();
            
            Assert.AreEqual(1, BarDecorator.Instances);
        }

        [TestMethod]
        public void GetInstance_PerScopeInjectecIntoTwoDifferentClasses_DoesNotReapplyDecorators()
        {
            BarDecorator.Instances = 0;
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Decorate<IBar, BarDecorator>();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IFoo, AnotherFooWithDependency>("AnotherFooWithDependency");

            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
                container.GetInstance<IFoo>("AnotherFooWithDependency");
            }
            Assert.AreEqual(1, BarDecorator.Instances);
        }

        [TestMethod]
        public void GetAllInstances_PerScopeInjectecIntoTwoDifferentClasses_DoesNotReapplyDecorators()
        {
            BarDecorator.Instances = 0;
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Decorate<IBar, BarDecorator>();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IFoo, AnotherFooWithDependency>("AnotherFooWithDependency");

            using (container.BeginScope())
            {
                container.GetAllInstances<IFoo>().ToList();
            }
            Assert.AreEqual(1, BarDecorator.Instances);
        }

        [TestMethod]
        public void GetInstance_ValueWithDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.RegisterInstance<IFoo>(new Foo());
            container.Decorate<IFoo, FooDecorator>();

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_ValueWithDecoratorRequestedTwice_DoesNotReapplyDecorators()
        {
            BarDecorator.Instances = 0;
            var container = CreateContainer();
            container.RegisterInstance<IBar>(new Bar());
            container.Decorate<IBar, BarDecorator>();

            container.GetInstance<IBar>();
            container.GetInstance<IBar>();

            Assert.AreEqual(1, BarDecorator.Instances);
        }

        [TestMethod]
        public void GetInstance_ValueInjectedIntoTwoDifferentClasses_DoesNotReapplyDecorators()
        {
            BarDecorator.Instances = 0;
            var container = CreateContainer();
            container.RegisterInstance<IBar>(new Bar());
            container.Decorate<IBar, BarDecorator>();
            container.Register<IFoo, FooWithDependency>();
            container.Register<IFoo, AnotherFooWithDependency>("AnotherFooWithDependency");

            container.GetAllInstances<IFoo>();

            Assert.AreEqual(1, BarDecorator.Instances);
        }

        [TestMethod]
        public void GetInstance_DecoratorAppliedToFunctionFactory_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => CreateFoo());
            container.Decorate(typeof(IFoo), typeof(FooDecorator));
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_DecoratorAppliedToDependencyViaFunctionFactory_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(factory => CreateFooWithDependency(factory));
            container.Register<IBar, Bar>();
            container.Decorate(typeof(IBar), typeof(BarDecorator));
            var instance = (FooWithDependency)container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance.Bar, typeof(BarDecorator));
        }

        [TestMethod]
        public void GetInstance_DeferredDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var registration = new DecoratorRegistration();            
            registration.CanDecorate = serviceRegistration => true;
            registration.ImplementingTypeFactory = (factory, serviceRegistration) => typeof(FooDecorator);
            container.Decorate(registration);

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_ClassWithConstructorArguments_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i));
            container.Decorate(typeof(IFoo), typeof(FooDecorator));

            var instance = container.GetInstance<int, IFoo>(42);

            Assert.IsInstanceOfType(instance, typeof(FooDecorator));

        }

        [TestMethod]
        public void GetInstance_ClassWithConstructorArgumentsAndLazyDecorator_CanCrTCreeateTarget()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i));
            container.Decorate<IFoo>((factory, foo) => new LazyFooDecorator(new Lazy<IFoo>(() => foo)));

            var instance = (LazyFooDecorator)container.GetInstance<int, IFoo>(42);

            Assert.IsNotNull(instance.Foo.Value);

        }





        private IFoo CreateFooWithDependency(IServiceFactory factory)
        {
            return new FooWithDependency(factory.GetInstance<IBar>());
        }

        private static Foo CreateFoo()
        {
            return new Foo();
        }

        private static FooDecorator GetFooDecorator(IFoo target)
        {
            return new FooDecorator(target);
        }
    }
}