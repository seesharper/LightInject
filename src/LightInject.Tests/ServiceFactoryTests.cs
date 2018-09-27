using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class ServiceFactoryTests : TestBase
    {
        [Fact]
        public void ShouldGetServiceUsingNonGenericFactory()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), factory => new Foo());

            var instance = container.GetInstance<IFoo>();

            Assert.IsType<Foo>(instance);
        }

        [Fact]
        public void ShouldGetNamedServiceUsingNonGenericFactory()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), factory => new Foo(), "SomeFoo");

            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.IsType<Foo>(instance);
        }

        [Fact]
        public void ShouldGetServiceWithLifetimeUsingNonGenericFactory()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), factory => new Foo(), new PerContainerLifetime());

            var firstInstance = container.GetInstance<IFoo>();
            var secondInstance = container.GetInstance<IFoo>();
            Assert.Same(firstInstance, secondInstance);
        }

        [Fact]
        public void ShouldGetNamedServiceWithLifetimeUsingNonGenericFactory()
        {
            var container = CreateContainer();
            container.Register(typeof(IFoo), factory => new Foo(), "SomeFoo", new PerContainerLifetime());

            var firstInstance = container.GetInstance<IFoo>("SomeFoo");
            var secondInstance = container.GetInstance<IFoo>("SomeFoo");
            Assert.Same(firstInstance, secondInstance);
        }
    }

}