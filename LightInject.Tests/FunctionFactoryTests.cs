namespace LightInject.Tests
{
    using System;

    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FunctionFactoryTests : TestBase
    {
        [TestMethod] 
        public void GetInstance_NoParameters_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo());
            var instance = container.GetInstance<Func<IFoo>>();
            Assert.IsInstanceOfType(instance, typeof(Func<IFoo>));
        }

        [TestMethod]
        public void GetInstance_NoParameters_ReturnsFactoryInstanceAsSingleton()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo());
            var firstInstance = container.GetInstance<Func<IFoo>>();
            var secondInstance = container.GetInstance<Func<IFoo>>();
            Assert.AreSame(firstInstance, secondInstance);
        }

        [TestMethod]
        public void GetInstance_OneParameter_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i));
            var instance = container.GetInstance<Func<int, IFoo>>();
            Assert.IsInstanceOfType(instance, typeof(Func<int, IFoo>));
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
        public void GetInstance_OneParameter_ThrowsMeaningfulExceptionWhenWrongGetInstanceMethodIsUsed()
        {
            
        }
      
        [TestMethod]
        public void GetInstance_OneValueTypeParameter_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i));
            var instance = (FooWithValueTypeDependency)container.GetInstance(typeof(IFoo), new object[] { 42 });
            Assert.AreEqual(42, instance.Value);
        }

        [TestMethod]
        public void GetInstance_OneReferenceTypeParameter_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<string, IFoo>((factory, s) => new FooWithReferenceTypeDependency(s));
            var instance = (FooWithReferenceTypeDependency)container.GetInstance(typeof(IFoo), new object[] { "SomeValue" });
            Assert.AreEqual("SomeValue", instance.Value);
        }
    }
}