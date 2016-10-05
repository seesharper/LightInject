namespace LightInject.Tests
{
    using System;
    using System.Linq;
    using SampleLibrary;
    using Xunit;
    using Xunit.Sdk;

    public class ScopeGetInstanceTests : TestBase
    {
        [Fact]
        public void ShouldGetInstanceFromScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            using (var scope = container.BeginScope())
            {
                var instance = scope.GetInstance<IFoo>();
                Assert.IsType<Foo>(instance);
            }
        }

        [Fact]
        public void ShouldGetDifferentInstancePerScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            using (var outerScope = container.BeginScope())
            {
                var firstInstance = outerScope.GetInstance<IFoo>();
                using (var innerScope = container.BeginScope())
                {
                    var secondInstance = innerScope.GetInstance<IFoo>();
                    Assert.NotSame(firstInstance, secondInstance);
                }
            }
        }

        [Fact]
        public void ShouldGetInstanceFromOuterScopeWhenCalledFromInnerScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            using (var outerScope = container.BeginScope())
            {
                var firstInstance = outerScope.GetInstance<IFoo>();
                using (container.BeginScope())
                {
                    var secondInstance = outerScope.GetInstance<IFoo>();
                    Assert.Same(firstInstance, secondInstance);
                }
            }
        }

        [Fact]
        public void ShouldGetNamedServiceFromScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("Foo", new PerScopeLifetime());
            using (var scope = container.BeginScope())
            {
                var instance = scope.GetInstance<IFoo>("Foo");
                Assert.IsType<Foo>(instance);
            }
        }

        [Fact]
        public void ShouldGetAllInstanceFromScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            container.Register<IFoo, AnotherFoo>("AnorherFoo", new PerScopeLifetime());
            using (var scope = container.BeginScope())
            {
                var instances = scope.GetAllInstances<IFoo>();
                Assert.Equal(2, instances.Count());
            }
        }

        [Fact]
        public void ShouldGetAllInstanceNonGenericFromScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            container.Register<IFoo, AnotherFoo>("AnorherFoo", new PerScopeLifetime());
            using (var scope = container.BeginScope())
            {
                var instances = scope.GetAllInstances(typeof(IFoo));
                Assert.Equal(2, instances.Count());
            }
        }


        [Fact]
        public void ShouldGetServiceWithRuntimeArgumentsFromScope()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, arg) => new FooWithValueTypeDependency(arg));
            using (var scope = container.BeginScope())
            {
                var instance = scope.GetInstance<int,IFoo>(42);
                Assert.IsType<FooWithValueTypeDependency>(instance);
            }
        }

        [Fact]
        public void ShouldGetNamedServiceWithRuntimeArgumentsFromScope()
        {
            var container = CreateContainer();
            container.Register<int, IFoo>((factory, arg) => new FooWithValueTypeDependency(arg), "Foo");
            using (var scope = container.BeginScope())
            {
                var instance = scope.GetInstance<int, IFoo>(42, "Foo");
                Assert.IsType<FooWithValueTypeDependency>(instance);
            }
        }

        [Fact]
        public void ShouldTryGetToGetServiceFromScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            using (var scope = container.BeginScope())
            {
                var instance = scope.TryGetInstance<IFoo>();
                Assert.IsType<Foo>(instance);
            }
        }

        [Fact]
        public void ShouldTryGetToGetNamedServiceFromScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("Foo");
            using (var scope = container.BeginScope())
            {
                var instance = scope.TryGetInstance<IFoo>("Foo");
                Assert.IsType<Foo>(instance);
            }
        }

        [Fact]
        public void ShouldCreateServiceFromScope()
        {
            var container = CreateContainer();
            using (var scope = container.BeginScope())
            {
                var instance = scope.Create<Foo>();
                Assert.IsType<Foo>(instance);
            }
        }

        [Fact]
        public void ShouldBeginScopeFromScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            using (var scope = container.BeginScope())
            {
                var firstInstance = scope.GetInstance<IFoo>();
                using (var innerScope = scope.BeginScope())
                {
                    var secondInstance = innerScope.GetInstance<IFoo>();
                    Assert.NotSame(firstInstance, secondInstance);
                }
            }
        }

        [Fact]
        public void ShouldThrowExceptionWhenEndingNonCurrentScope()
        {
            var container = CreateContainer();

            using (container.BeginScope())
            {
                var scope = new Scope(container.ScopeManagerProvider.GetScopeManager(container), null);
                Assert.Throws<InvalidOperationException>(() => scope.Dispose());
            }

        }
    }
}