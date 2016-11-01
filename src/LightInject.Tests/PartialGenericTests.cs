namespace LightInject.Tests
{
    using System;
    using SampleLibrary;
    using Xunit;

    public class PartialGenericTests : TestBase
    {
        [Fact]
        public void GetInstance_PartiallyClosedGeneric_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof (IFoo<,>), typeof (HalfClosedFoo<>));

            var instance = container.GetInstance<IFoo<string, int>>();

            Assert.IsType<HalfClosedFoo<int>>(instance);
        }

        [Fact]
        public void GetInstance_PartiallyClosedGenericWithNestedArgument_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<,>), typeof(HalfClosedFooWithNestedGenericParameter<>));

            var instance = container.GetInstance<IFoo<Lazy<int>, string>>();

            Assert.IsType<HalfClosedFooWithNestedGenericParameter<int>>(instance);
        }

        [Fact]
        public void GetInstance_PartiallyClosedGenericWithDoubleNestedArgument_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<,>), typeof(HalfClosedFooWithDoubleNestedGenericParameter<>));

            var instance = container.GetInstance<IFoo<Lazy<Nullable<int>>, string>>();

            Assert.IsType<HalfClosedFooWithDoubleNestedGenericParameter<int>>(instance);
        }

        [Fact]
        public void GetInstance_PartialClosedAbstractClass_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof (Foo<,>), typeof (HalfClosedFooInhertingFromBaseClass<>));

            var instance = container.GetInstance<Foo<string,int>>();

            Assert.IsType<HalfClosedFooInhertingFromBaseClass<int>>(instance);
        }



    }
}