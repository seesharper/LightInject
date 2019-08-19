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
                var instance = scope.GetInstance<int, IFoo>(42);
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
        public void ShouldUseInitialScopeWhenResolvingFunc()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithFuncDependency>(new PerScopeLifetime());
            using (var outerScope = container.BeginScope())
            {
                var foo = (FooWithFuncDependency)outerScope.GetInstance<IFoo>();
                var bar1 = outerScope.GetInstance<IBar>();
                var bar2 = foo.GetBar();
                Assert.Same(bar1, bar2);
                using (var innerScope = container.BeginScope())
                {
                    var bar3 = foo.GetBar();
                    Assert.Same(bar1, bar3);
                }
            }
        }

        [Fact]
        public void ShouldUseInitialScopeWhenResolvingFuncOverNamedService()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>("SomeBar", new PerScopeLifetime());
            container.Register<IFoo, FooWithNamedFuncDependency>(new PerScopeLifetime());
            using (var outerScope = container.BeginScope())
            {
                var foo = (FooWithNamedFuncDependency)outerScope.GetInstance<IFoo>();
                var bar1 = outerScope.GetInstance<IBar>();
                var bar2 = foo.GetBar("SomeBar");
                Assert.Same(bar1, bar2);
                using (var innerScope = container.BeginScope())
                {
                    var bar3 = foo.GetBar("SomeBar");
                    Assert.Same(bar1, bar3);
                }
            }
        }

        [Fact]
        public void ShouldUseInitialScopeWhenResolvingLazy()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithLazyDependency>(new PerScopeLifetime());
            using (var outerScope = container.BeginScope())
            {
                var foo = (FooWithLazyDependency)outerScope.GetInstance<IFoo>();
                var bar1 = outerScope.GetInstance<IBar>();
                using (var innerScope = container.BeginScope())
                {
                    var bar3 = foo.LazyService.Value;
                    Assert.Same(bar1, bar3);
                    var bar4 = innerScope.GetInstance<IBar>();
                    Assert.NotSame(bar1, bar4);
                }
            }
        }

        [Fact]
        public void ShouldUseInitialScopeWhenResolvingEnumerable()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IBar, AnotherBar>("AnotherBar", new PerScopeLifetime());
            container.Register<FooWithFuncOverEnumerable>();
            using (var outerScope = container.BeginScope())
            {
                var foo = outerScope.GetInstance<FooWithFuncOverEnumerable>();
                var bars1 = outerScope.GetAllInstances<IBar>();
                using (var innerScope = container.BeginScope())
                {
                    var bars2 = foo.BarsFunc();
                    Assert.True(bars1.SequenceEqual(bars2));
                    var bars3 = innerScope.GetAllInstances<IBar>();
                    Assert.NotEqual(bars2, bars3);
                }
            }
        }

        [Fact]
        public void ShouldPassAmbientScopeToFactory()
        {
            var container = CreateContainer();
            IServiceFactory passedFactory = null;

            container.Register<DisposableFoo>(f =>
                {
                    passedFactory = f;
                    return new DisposableFoo();
                });
            using (var scope = container.BeginScope())
            {
                var instance = container.GetInstance<DisposableFoo>();
                Assert.Same(scope, passedFactory);
            }
        }

        [Fact]
        public void ShouldPassScopeToFactory()
        {
            var container = CreateContainer();
            IServiceFactory passedFactory = null;

            container.Register<DisposableFoo>(f =>
                {
                    passedFactory = f;
                    return new DisposableFoo();
                });
            using (var scope = container.BeginScope())
            {
                var instance = scope.GetInstance<DisposableFoo>();
                Assert.Same(scope, passedFactory);
            }
        }

        [Fact]
        public void ShouldPassContainerToFactory()
        {
            var container = CreateContainer();
            IServiceFactory passedFactory = null;

            container.Register<DisposableFoo>(f =>
                {
                    passedFactory = f;
                    return new DisposableFoo();
                });

            var instance = container.GetInstance<DisposableFoo>();
            Assert.Same(container, passedFactory);
        }


        [Fact]
        public void ShouldPassScopeToFallback()
        {
            var container = CreateContainer();
            IServiceFactory passedFactory = null;
            container.RegisterFallback((t, s) => t == typeof(DisposableFoo), sr =>
            {
                passedFactory = sr.ServiceFactory;
                return new DisposableFoo();
            });

            using (var scope = container.BeginScope())
            {
                var instance = scope.GetInstance<DisposableFoo>();
                Assert.Same(scope, passedFactory);
            }
        }

        [Fact]
        public void ShouldPassScopeToConstructorDependency()
        {
            var container = CreateContainer();
            IServiceFactory passedFactory = null;
            container.Register<FooWithDependency>();
            container.RegisterConstructorDependency<IBar>((factory, paramater) =>
            {
                passedFactory = factory;
                return new Bar();
            });

            using (var scope = container.BeginScope())
            {
                var instance = scope.GetInstance<FooWithDependency>();
                Assert.Same(scope, passedFactory);
            }

        }
    }


}