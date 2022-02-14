using System;
using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class CloneableLifetimeTests : TestBase
    {
        [Fact]
        public void ShouldClonePerContainerLifetime()
        {
            var lifetime = new PerContainerLifetime();

            var clone = lifetime.Clone();

            Assert.IsType<PerContainerLifetime>(clone);
        }

        [Fact]
        public void ShouldClonePerScopeLifetime()
        {
            var lifetime = new PerScopeLifetime();

            var clone = lifetime.Clone();

            Assert.IsType<PerScopeLifetime>(clone);
        }

        [Fact]
        public void ShouldClonePerRequestLifetime()
        {
            var lifetime = new PerRequestLifeTime();

            var clone = lifetime.Clone();

            Assert.IsType<PerRequestLifeTime>(clone);
        }

        [Fact]
        public void ShouldHandleOpenGenericTypeWithCloneableLifetime()
        {
            var container = new ServiceContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), new PerContainerLifetime());

            var instance = container.GetInstance<IFoo<string>>();

            Assert.IsType<Foo<string>>(instance);
        }


        [Fact]
        public void ShouldHandleOpenGenericTypeWithNonCloneableLifetime()
        {
            var container = new ServiceContainer();
            container.Register(typeof(IFoo<>), typeof(Foo<>), new LifetimeWithoutCloneableImplementation());

            var instance = container.GetInstance<IFoo<string>>();

            Assert.IsType<Foo<string>>(instance);
        }

        internal class LifetimeWithoutCloneableImplementation : ILifetime
        {
            public object GetInstance(Func<object> createInstance, Scope scope)
            {
                return createInstance();
            }
        }
    }


}
