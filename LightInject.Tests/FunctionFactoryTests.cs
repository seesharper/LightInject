namespace LightInject.Tests
{
    using System;

    using LightInject.SampleLibrary;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

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
        public void GetInstance_ThreeParameters_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            var instance = container.GetInstance<Func<int, int, int, IFoo>>();
            Assert.IsInstanceOfType(instance, typeof(Func<int, int, int, IFoo>));
        }

        [TestMethod]
        public void GetInstance_FourParameters_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            var instance = container.GetInstance<Func<int, int, int, int, IFoo>>();
            Assert.IsInstanceOfType(instance, typeof(Func<int, int, int, int, IFoo>));
        }


        [TestMethod]
        public void Invoke_OneParameter_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithOneParameter(i));
            var fooFactory = container.GetInstance<Func<int, IFoo>>();
            var instance = (FooWithOneParameter)fooFactory(1);
            Assert.AreEqual(1, instance.Arg1);
        }
        
        [TestMethod]
        public void Invoke_TwoParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, IFoo>((factory, arg1, arg2) => new FooWithTwoParameters(arg1, arg2));
            var fooFactory = container.GetInstance<Func<int, int, IFoo>>();
            var instance = (FooWithTwoParameters)fooFactory(1, 2);
            Assert.AreEqual(1, instance.Arg1);
            Assert.AreEqual(2, instance.Arg2);
        }

        [TestMethod]
        public void Invoke_ThreeParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, int, IFoo>((factory, arg1, arg2, arg3 ) => new FooWithThreeParameters(arg1, arg2, arg3));
            var fooFactory = container.GetInstance<Func<int, int, int, IFoo>>();
            var instance = (FooWithThreeParameters)fooFactory(1, 2, 3);
            Assert.AreEqual(1, instance.Arg1);
            Assert.AreEqual(2, instance.Arg2);
            Assert.AreEqual(3, instance.Arg3);
        }

        [TestMethod]
        public void Invoke_FourParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, int, int, IFoo>((factory, arg1, arg2, arg3, arg4) => new FooWithFourParameters(arg1, arg2, arg3, arg4));
            var fooFactory = container.GetInstance<Func<int, int, int, int, IFoo>>();
            var instance = (FooWithFourParameters)fooFactory(1, 2, 3, 4);
            Assert.AreEqual(1, instance.Arg1);
            Assert.AreEqual(2, instance.Arg2);
            Assert.AreEqual(3, instance.Arg3);
            Assert.AreEqual(4, instance.Arg4);
        }

        [TestMethod]
        public void Invoke_NamedOneParameter_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithOneParameter(i), "SomeFoo");
            var fooFactory = container.GetInstance<Func<int, IFoo>>("SomeFoo");
            var instance = (FooWithOneParameter)fooFactory(1);
            Assert.AreEqual(1, instance.Arg1);
        }

        [TestMethod]
        public void Invoke_NamedTwoParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, IFoo>((factory, arg1, arg2) => new FooWithTwoParameters(arg1, arg2), "SomeFoo");
            var fooFactory = container.GetInstance<Func<int, int, IFoo>>("SomeFoo");
            var instance = (FooWithTwoParameters)fooFactory(1, 2);
            Assert.AreEqual(1, instance.Arg1);
            Assert.AreEqual(2, instance.Arg2);
        }

        [TestMethod]
        public void Invoke_NamedThreeParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, int, IFoo>((factory, arg1, arg2, arg3) => new FooWithThreeParameters(arg1, arg2, arg3), "SomeFoo");
            var fooFactory = container.GetInstance<Func<int, int, int, IFoo>>("SomeFoo");
            var instance = (FooWithThreeParameters)fooFactory(1, 2, 3);
            Assert.AreEqual(1, instance.Arg1);
            Assert.AreEqual(2, instance.Arg2);
            Assert.AreEqual(3, instance.Arg3);
        }

        [TestMethod]
        public void Invoke_NamedFourParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, int, int, IFoo>((factory, arg1, arg2, arg3, arg4) => new FooWithFourParameters(arg1, arg2, arg3, arg4), "SomeFoo");
            var fooFactory = container.GetInstance<Func<int, int, int, int, IFoo>>("SomeFoo");
            var instance = (FooWithFourParameters)fooFactory(1, 2, 3, 4);
            Assert.AreEqual(1, instance.Arg1);
            Assert.AreEqual(2, instance.Arg2);
            Assert.AreEqual(3, instance.Arg3);
            Assert.AreEqual(4, instance.Arg4);
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

        //[TestMethod]
        //public void GetInstance_WithTooFewArguments_ThrowsMeaningfulException()
        //{
        //    var container = CreateContainer();
        //    container.Register<int, int, IFoo>((factory, arg1, arg2) => new FooWithTwoParameters(arg1, arg2), "SomeFoo");
        //    ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo), new object[] { 42 }));
        //}
    }
}