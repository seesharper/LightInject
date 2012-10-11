namespace LightInject.Tests
{
    using System.Linq;

    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DecoratorTests
    {
        [TestMethod]
        public void GetInstance_WithDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate(typeof(IFoo), typeof(FooDecorator), si => true);
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        [TestMethod]
        public void GetInstance_WithNestedDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();            
            container.Decorate(typeof(IFoo), typeof(FooDecorator), si => true);
            container.Decorate(typeof(IFoo), typeof(AnotherFooDecorator), si => true);
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFooDecorator));
        }

        [TestMethod]
        public void GetAllInstances_WithDecorator_ReturnsDecoratedInstances()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, AnotherFoo>("AnotherFoo");
            container.Decorate(typeof(IFoo), typeof(FooDecorator), si => true);
            var instances = container.GetAllInstances<IFoo>();
            Assert.IsInstanceOfType(instances.First(), typeof(FooDecorator));
            Assert.IsInstanceOfType(instances.Last(), typeof(FooDecorator));
        }


        [TestMethod]
        public void GetInstance_WithOpenGenericDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>), si => true);
            var instance = container.GetInstance<IFoo<int>>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator<int>));
        }

        [TestMethod]
        public void GetInstance_WithNestedOpenGenericDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>), si => true);
            container.Decorate(typeof(IFoo<>), typeof(AnotherFooDecorator<>), si => true);
            var instance = container.GetInstance<IFoo<int>>();
            Assert.IsInstanceOfType(instance, typeof(AnotherFooDecorator<int>));
        }

        [TestMethod]
        public void GetInstance_ClosedGenericServiceWithOpenGenericDecorator_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<int>), typeof(Foo<int>));
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>), si => true);
            var instance = container.GetInstance<IFoo<int>>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator<int>));
        }

        [TestMethod]
        public void GetAllInstances_WithOpenGenericDecorator_ReturnsDecoratedInstances()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>));
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");
            container.Decorate(typeof(IFoo<>), typeof(FooDecorator<>), si => true);
            var instances = container.GetAllInstances<IFoo<int>>();
            Assert.IsInstanceOfType(instances.First(), typeof(FooDecorator<int>));
            Assert.IsInstanceOfType(instances.Last(), typeof(FooDecorator<int>));
        }
        



        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        } 
    }


}