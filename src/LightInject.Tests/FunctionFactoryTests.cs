using System;
using LightInject.SampleLibrary;
using Xunit;
namespace LightInject.Tests
{
    public class FunctionFactoryTests : TestBase
    {
        [Fact]
        public void Invoke_NoParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo());
            var fooFactory = container.GetInstance<Func<IFoo>>();
            var instance = fooFactory();
            Assert.IsAssignableFrom<Foo>(instance);
        }

        [Fact]
        public void Invoke_NamedNoParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo(), "SomeFoo");
            var fooFactory = container.GetInstance<Func<IFoo>>("SomeFoo");
            var instance = fooFactory();
            Assert.IsAssignableFrom<Foo>(instance);
        }

        [Fact]
        public void GetInstance_NamedNoParameters_ReturnsFactoryInstanceAsTransients()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo(), "SomeFoo");
            var firstInstance = container.GetInstance<Func<IFoo>>("SomeFoo");
            var secondInstance = container.GetInstance<Func<IFoo>>("SomeFoo");
            Assert.NotSame(firstInstance, secondInstance);
        }

        [Fact]
        public void GetInstance_TwoServices_FactoriesAreNotSame()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new Foo(), "SomeFoo");
            container.Register<IFoo>(factory => new Foo(), "AnotherFoo");
            var firstInstance = container.GetInstance<Func<IFoo>>("SomeFoo");
            var secondInstance = container.GetInstance<Func<IFoo>>("AnotherFoo");
            Assert.NotSame(firstInstance, secondInstance);
        }

        [Fact]
        public void GetInstance_NoParameters_ReturnsFactoryInstanceAsTransients()
        {
            var container = CreateContainer();
            var firstInstance = container.GetInstance<Func<IFoo>>();
            var secondInstance = container.GetInstance<Func<IFoo>>();
            Assert.NotSame(firstInstance, secondInstance);
        }

        [Fact]
        public void GetInstance_OneParameter_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            var instance = container.GetInstance<Func<int, IFoo>>();
            Assert.IsAssignableFrom<Func<int, IFoo>>(instance);
        }

        [Fact]
        public void GetInstance_TwoParameters_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            var instance = container.GetInstance<Func<int, int, IFoo>>();
            Assert.IsAssignableFrom<Func<int, int, IFoo>>(instance);
        }

        [Fact]
        public void GetInstance_ThreeParameters_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            var instance = container.GetInstance<Func<int, int, int, IFoo>>();
            Assert.IsAssignableFrom<Func<int, int, int, IFoo>>(instance);
        }

        [Fact]
        public void GetInstance_FourParameters_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            var instance = container.GetInstance<Func<int, int, int, int, IFoo>>();
            Assert.IsAssignableFrom<Func<int, int, int, int, IFoo>>(instance);
        }

        [Fact]
        public void Invoke_OneParameter_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithOneParameter(i));
            var fooFactory = container.GetInstance<Func<int, IFoo>>();
            var instance = (FooWithOneParameter)fooFactory(1);
            Assert.Equal(1, instance.Arg1);
        }

        [Fact]
        public void Invoke_TwoParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, IFoo>((factory, arg1, arg2) => new FooWithTwoParameters(arg1, arg2));
            var fooFactory = container.GetInstance<Func<int, int, IFoo>>();
            var instance = (FooWithTwoParameters)fooFactory(1, 2);
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
        }

        [Fact]
        public void Invoke_ThreeParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, int, IFoo>((factory, arg1, arg2, arg3) => new FooWithThreeParameters(arg1, arg2, arg3));
            var fooFactory = container.GetInstance<Func<int, int, int, IFoo>>();
            var instance = (FooWithThreeParameters)fooFactory(1, 2, 3);
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
            Assert.Equal(3, instance.Arg3);
        }

        [Fact]
        public void Invoke_FourParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, int, int, IFoo>((factory, arg1, arg2, arg3, arg4) => new FooWithFourParameters(arg1, arg2, arg3, arg4));
            var fooFactory = container.GetInstance<Func<int, int, int, int, IFoo>>();
            var instance = (FooWithFourParameters)fooFactory(1, 2, 3, 4);
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
            Assert.Equal(3, instance.Arg3);
            Assert.Equal(4, instance.Arg4);
        }

        [Fact]
        public void Invoke_NamedOneParameter_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithOneParameter(i), "SomeFoo");
            var fooFactory = container.GetInstance<Func<int, IFoo>>("SomeFoo");
            var instance = (FooWithOneParameter)fooFactory(1);
            Assert.Equal(1, instance.Arg1);
        }

        [Fact]
        public void Invoke_NamedTwoParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, IFoo>((factory, arg1, arg2) => new FooWithTwoParameters(arg1, arg2), "SomeFoo");
            var fooFactory = container.GetInstance<Func<int, int, IFoo>>("SomeFoo");
            var instance = (FooWithTwoParameters)fooFactory(1, 2);
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
        }

        [Fact]
        public void Invoke_NamedThreeParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, int, IFoo>((factory, arg1, arg2, arg3) => new FooWithThreeParameters(arg1, arg2, arg3), "SomeFoo");
            var fooFactory = container.GetInstance<Func<int, int, int, IFoo>>("SomeFoo");
            var instance = (FooWithThreeParameters)fooFactory(1, 2, 3);
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
            Assert.Equal(3, instance.Arg3);
        }

        [Fact]
        public void Invoke_NamedFourParameters_FactoryCreatesInstanceAndPassesValuesToConstructor()
        {
            var container = CreateContainer();
            container.Register<int, int, int, int, IFoo>((factory, arg1, arg2, arg3, arg4) => new FooWithFourParameters(arg1, arg2, arg3, arg4), "SomeFoo");
            var fooFactory = container.GetInstance<Func<int, int, int, int, IFoo>>("SomeFoo");
            var instance = (FooWithFourParameters)fooFactory(1, 2, 3, 4);
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
            Assert.Equal(3, instance.Arg3);
            Assert.Equal(4, instance.Arg4);
        }

        [Fact]
        public void GetInstance_OneParameter_ReturnsFactoryInstanceAsSingleton()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i));
            var firstInstance = container.GetInstance<Func<int, IFoo>>();
            var secondInstance = container.GetInstance<Func<int, IFoo>>();
            Assert.Same(firstInstance, secondInstance);
        }

        [Fact]
        public void GetInstance_NamedService_ReturnsFactoryInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i), "SomeFoo");
            var instance = container.GetInstance<Func<int, IFoo>>("SomeFoo");
            Assert.IsAssignableFrom<Func<int, IFoo>>(instance);
        }

        [Fact]
        public void GetInstance_TwoNamedServices_FactoriesReturnsNamedServices()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i), "SomeFoo");
            container.Register<int, IFoo>((factory, i) => new AnotherFooWithValueTypeDependency(i), "AnotherFoo");
            var firstFactory = container.GetInstance<Func<int, IFoo>>("SomeFoo");
            var instance = firstFactory(42);
            Assert.IsAssignableFrom<FooWithValueTypeDependency>(instance);
        }

        [Fact]
        public void GetInstance_OneParameter_ThrowsMeaningfulExceptionWhenWrongGetInstanceMethodIsUsed()
        {

        }

        [Fact]
        public void GetInstance_ValueTypeParameter_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i));
            var instance = (FooWithValueTypeDependency)container.GetInstance(typeof(IFoo), new object[] { 42 });
            Assert.Equal(42, instance.Value);
        }

        [Fact]
        public void GetInstance_ReferenceTypeParameter_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<string, IFoo>((factory, s) => new FooWithReferenceTypeDependency(s));
            var instance = (FooWithReferenceTypeDependency)container.GetInstance(typeof(IFoo), new object[] { "SomeValue" });
            Assert.Equal("SomeValue", instance.Value);
        }

        [Fact]
        public void GetInstance_NamedService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<string, IFoo>((factory, s) => new FooWithReferenceTypeDependency(s), "SomeFoo");
            var instance = (FooWithReferenceTypeDependency)container.GetInstance(typeof(IFoo), "SomeFoo", new object[] { "SomeValue" });
            Assert.Equal("SomeValue", instance.Value);
        }

        [Fact]
        public void GetInstance_TwoNamedServices_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithValueTypeDependency(i), "SomeFoo");
            container.Register<int, IFoo>((factory, i) => new AnotherFooWithValueTypeDependency(i), "AnotherFoo");
            var instance = (FooWithValueTypeDependency)container.GetInstance(typeof(IFoo), "SomeFoo", new object[] { 42 });
            var instance2 = (AnotherFooWithValueTypeDependency)container.GetInstance(typeof(IFoo), "AnotherFoo", new object[] { 42 });
            Assert.Equal(42, instance.Value);
        }

        //[Fact]
        //public void Ex()
        //{
        //    var container = CreateContainer();
        //    container.Register<IFoo, FooWithValueTypeDependency>(new PerContainerLifetime());
        //    var instance = container.GetInstance<int, IFoo>(42);
        //    var instance2 = container.GetInstance<int, IFoo>(84);
        //}



        [Fact]
        public void GetInstance_OneParameter_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithOneParameter(i));
            var instance = (FooWithOneParameter)container.GetInstance<int, IFoo>(1);
            Assert.Equal(1, instance.Arg1);
        }

        [Fact]
        public void GetInstance_NamedOneParameter_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, i) => new FooWithOneParameter(i), "SomeFoo");
            var instance = (FooWithOneParameter)container.GetInstance<int, IFoo>(1, "SomeFoo");
            Assert.Equal(1, instance.Arg1);
        }

        [Fact]
        public void GetInstance_TwoParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, int, IFoo>((factory, i1, i2) => new FooWithTwoParameters(i1, i2));
            var instance = (FooWithTwoParameters)container.GetInstance<int, int, IFoo>(1, 2);
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
        }

        [Fact]
        public void GetInstance_NamedTwoParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, int, IFoo>((factory, i1, i2) => new FooWithTwoParameters(i1, i2), "SomeFoo");
            var instance = (FooWithTwoParameters)container.GetInstance<int, int, IFoo>(1, 2, "SomeFoo");
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
        }

        [Fact]
        public void GetInstance_ThreeParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, int, int, IFoo>((factory, i1, i2, i3) => new FooWithThreeParameters(i1, i2, i3));
            var instance = (FooWithThreeParameters)container.GetInstance<int, int, int, IFoo>(1, 2, 3);
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
            Assert.Equal(3, instance.Arg3);
        }

        [Fact]
        public void GetInstance_NamedThreeParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, int, int, IFoo>((factory, i1, i2, i3) => new FooWithThreeParameters(i1, i2, i3), "SomeFoo");
            var instance = (FooWithThreeParameters)container.GetInstance<int, int, int, IFoo>(1, 2, 3, "SomeFoo");
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
            Assert.Equal(3, instance.Arg3);
        }

        [Fact]
        public void GetInstance_FourParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, int, int, int, IFoo>((factory, i1, i2, i3, i4) => new FooWithFourParameters(i1, i2, i3, i4));
            var instance = (FooWithFourParameters)container.GetInstance<int, int, int, int, IFoo>(1, 2, 3, 4);
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
            Assert.Equal(3, instance.Arg3);
            Assert.Equal(4, instance.Arg4);
        }

        [Fact]
        public void GetInstance_NamedFourParameters_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<int, int, int, int, IFoo>((factory, i1, i2, i3, i4) => new FooWithFourParameters(i1, i2, i3, i4), "SomeFoo");
            var instance = (FooWithFourParameters)container.GetInstance<int, int, int, int, IFoo>(1, 2, 3, 4, "SomeFoo");
            Assert.Equal(1, instance.Arg1);
            Assert.Equal(2, instance.Arg2);
            Assert.Equal(3, instance.Arg3);
            Assert.Equal(4, instance.Arg4);
        }

        [Fact]
        public void GetInstance_MissingConstructorDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo>(factory => new FooWithDependency(factory.GetInstance<IBar>()));
            Assert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());
        }



        //[Fact]
        //public void GetAllInstance_FunctionFactoryWithParameters_DoesNotReturnAnyServices()
        //{
        //    var container = CreateContainer();
        //    container.Register<int, IFoo>((factory, i) => new FooWithOneParameter(i));

        //    var services = container.GetAllInstances<IFoo>();

        //    Assert.Equal(0, services.Count());

        //}



        //[Fact]
        //public void GetInstance_WithTooFewArguments_ThrowsMeaningfulException()
        //{
        //    var container = CreateContainer();
        //    container.Register<int, int, IFoo>((factory, arg1, arg2) => new FooWithTwoParameters(arg1, arg2), "SomeFoo");
        //    ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance(typeof(IFoo), new object[] { 42 }));
        //}
    }
}