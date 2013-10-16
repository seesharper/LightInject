namespace LightInject.Tests
{
    using System;

    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FunctionFactoryTests : TestBase
    {        
        [TestMethod]
        public void Invoke_NoParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo());
            var fooFactory = container.GetInstance<Func<IFoo>>();
            var instance = fooFactory();
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void Invoke_NamedNoParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo(), "SomeFoo");
            var fooFactory = container.GetInstance<Func<IFoo>>("SomeFoo");
            var instance = fooFactory();
            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetInstance_NoParameters_ReturnsFactoryInstanceAsSingleton()
        {
            var container = CreateContainer();            
            var firstInstance = container.GetInstance<Func<IFoo>>();
            var secondInstance = container.GetInstance<Func<IFoo>>();
            Assert.AreSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_NamedNoParameters_ReturnsFactoryInstanceAsSingleton()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo(), "SomeFoo");
            var firstInstance = container.GetInstance<Func<IFoo>>("SomeFoo");
            var secondInstance = container.GetInstance<Func<IFoo>>("SomeFoo");
            Assert.AreSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_TwoServices_FactoriesAreNotSame()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo(), "SomeFoo");
            container.Register<IFoo>(factory => new Foo(), "AnotherFoo");
            var firstInstance = container.GetInstance<Func<IFoo>>("SomeFoo");
            var secondInstance = container.GetInstance<Func<IFoo>>("AnotherFoo");
            Assert.AreNotSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_OneParameter_ReturnsFactoryInstance()
        {
            var container = CreateContainer();            
            var instance = container.GetInstance<Func<int, IFoo>>();
            Assert.IsInstanceOfType(instance, typeof(Func<int, IFoo>));
        }
        [TestMethod]
        public void GetInstance_TwoParameters_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            var instance = container.GetInstance<Func<int, int, IFoo>>();
            Assert.IsInstanceOfType(instance, typeof(Func<int, int, IFoo>));
        }


        [TestMethod]
        public void GetInstance_OneParameter_FactoryCanCreateInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i));
            var fooFactory = container.GetInstance<Func<int, IFoo>>();
            var instance = fooFactory(42);
            Assert.IsInstanceOfType(instance, typeof(FooWithValueTypeDependency));

        }


        [TestMethod]
        public void GetInstance_OneParameter_ReturnsFactoryInstanceAsSingleton()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i));
            var firstInstance = container.GetInstance<Func<int, IFoo>>();
            var secondInstance = container.GetInstance<Func<int, IFoo>>();
            Assert.AreSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_NamedService_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i), "SomeFoo");
            var instance = container.GetInstance<Func<int, IFoo>>("SomeFoo");
            Assert.IsInstanceOfType(instance, typeof(Func<int, IFoo>));
        }

        [TestMethod]
        public void GetInstance_TwoNamedServices_FactoriesReturnsNamedServices()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i), "SomeFoo");
            container.Register<int, IFoo>((factory, i) => new AnotherFooWithValueTypeDependency(i), "AnotherFoo");
            var firstFactory = container.GetInstance<Func<int, IFoo>>("SomeFoo");
            var instance = firstFactory(42);
            Assert.IsInstanceOfType(instance, typeof(FooWithValueTypeDependency));            
        }

        [TestMethod]
        public void GetInstance_OneParameter_ThrowsMeaningfulExceptionWhenWrongGetInstanceMethodIsUsed()
        {
                
        }
      
        [TestMethod]
        public void GetInstance_ValueTypeParameter_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i));
            var instance = (FooWithValueTypeDependency)container.GetInstance(typeof(IFoo), new object[] { 42 });
            Assert.AreEqual(42, instance.Value);
        }

        [TestMethod]
        public void GetInstance_ReferenceTypeParameter_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<string, IFoo>((factory, s) => new FooWithReferenceTypeDependency(s));
            var instance = (FooWithReferenceTypeDependency)container.GetInstance(typeof(IFoo), new object[] { "SomeValue" });
            Assert.AreEqual("SomeValue", instance.Value);
        }

        [TestMethod]
        public void GetInstance_NamedService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<string, IFoo>((factory, s) => new FooWithReferenceTypeDependency(s), "SomeFoo");
            var instance = (FooWithReferenceTypeDependency)container.GetInstance(typeof(IFoo), "SomeFoo", new object[] { "SomeValue" });
            Assert.AreEqual("SomeValue", instance.Value);
        }

        [TestMethod]
        public void GetInstance_TwoNamedServices_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i), "SomeFoo");
            container.Register<int, IFoo>((factory, i) => new AnotherFooWithValueTypeDependency(i), "AnotherFoo");
            var instance = (FooWithValueTypeDependency)container.GetInstance(typeof(IFoo), "SomeFoo", new object[] { 42 });
            var instance2 = (AnotherFooWithValueTypeDependency)container.GetInstance(typeof(IFoo), "AnotherFoo", new object[] { 42 });
            Assert.AreEqual(42, instance.Value);
        }
    }
}