using System;
using System.Reflection;
using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class OpenGenericTests : TestBase
    {
        [Fact]
        public void GetInstance_PartiallyClosedGeneric_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<,>), typeof(HalfClosedFoo<>));

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
            container.Register(typeof(Foo<,>), typeof(HalfClosedFooInhertingFromBaseClass<>));

            var instance = container.GetInstance<Foo<string, int>>();

            Assert.IsType<HalfClosedFooInhertingFromBaseClass<int>>(instance);
        }

        [Fact]
        public void GetInstance_ConcreteClass_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(Foo<>));

            var instance = container.GetInstance<Foo<int>>();

            Assert.IsType<Foo<int>>(instance);
        }

        [Fact]
        public void GetInstance_InheritFromOpenGeneric_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register(typeof(GenericFoo<>), typeof(AnotherGenericFoo<>));

            var instance = container.GetInstance<GenericFoo<int>>();

            Assert.IsType<AnotherGenericFoo<int>>(instance);
        }

        [Fact]
        public void GetInstance_NamedServiceWithInvalidConstraint_ThrowsException()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericConstraint<>), "SomeService");

            Assert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo<int>>("SomeService"));
        }

        [Fact]
        public void GetInstance_NoMatchingOpenGeneric_ThrowsException()
        {
            var container = CreateContainer();

            Assert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo<int>>());

        }

        [Fact]
        public void GetInstance_NamedOpenGenerics_IgnoresCaseOnServiceNames()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), "SomeFoo");
            container.Register(typeof(IFoo<>), typeof(AnotherFoo<>), "AnotherFoo");

            var instance = container.GetInstance<IFoo<int>>("somefoo");

            Assert.IsType<Foo<int>>(instance);
        }

        [Fact]
        public void ShouldMapNestGenericArguments()
        {
            var container = CreateContainer();

            container.Register(typeof(IHandler<>), typeof(Handler<>), "Handler");
            container.Register(typeof(IHandler<>), typeof(AnotherHandler<>), "AnotherHandler");

            var handlerInstance = container.GetInstance<IHandler<Message<string>>>();
            Assert.IsType<Handler<string>>(handlerInstance);
            var anotherHandlerInstance = container.GetInstance<IHandler<AnotherMessage<string>>>();
            Assert.IsType<AnotherHandler<string>>(anotherHandlerInstance);
        }
    }

    public interface IHandler<TCommand>
    {
    }

    public class Message<TMessage>
    {

    }

    public class AnotherMessage<TAnotherMessage>
    {

    }

    public class Handler<TCommand> : IHandler<Message<TCommand>>
    {
        public Handler()
        {

        }
    }

    public class AnotherHandler<TCommand> : IHandler<AnotherMessage<TCommand>>
    {
        public AnotherHandler()
        {

        }
    }
}