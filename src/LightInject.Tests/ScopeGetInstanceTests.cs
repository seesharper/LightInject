namespace LightInject.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
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
        public void ShouldGetTransientFromScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, DisposableFoo>(new PerRequestLifeTime());
            IFoo firstFoo;
            IFoo secondFoo;
            using (var scope = container.BeginScope())
            {
                firstFoo = scope.GetInstance<IFoo>();
                secondFoo = scope.GetInstance<IFoo>();
                Assert.NotSame(firstFoo, secondFoo);
            }

            Assert.True(((DisposableFoo)firstFoo).IsDisposed);
            Assert.True(((DisposableFoo)secondFoo).IsDisposed);
        }


        [Fact]
        public void ShouldThrowMeaningfulMessageWhenScopedInstanceIsRequestFromContainer()
        {
            var container = CreateContainer(new ContainerOptions() { EnableCurrentScope = false });
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            using (var scope = container.BeginScope())
            {
                var exception = Assert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());
                Assert.StartsWith("Attempt to create a scoped instance", exception.Message);
            }
        }

        [Fact]
        public void ShouldGEtDefaultAndNamedServiceFromScope()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            container.Register<IFoo, AnotherFoo>("AnotherFoo", new PerScopeLifetime());

            using (var scope = container.BeginScope())
            {
                var foo = scope.GetInstance<IFoo>();
                Assert.IsType<Foo>(foo);
                var anotherFoo = scope.GetInstance<IFoo>("AnotherFoo");
                Assert.IsType<AnotherFoo>(anotherFoo);
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
        public void ShouldUseInitialScopeWhenResolvingFuncUsingTryGetInstance()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithFuncDependency>(new PerScopeLifetime());
            using (var outerScope = container.BeginScope())
            {
                var foo = (FooWithFuncDependency)outerScope.TryGetInstance<IFoo>();
                var bar1 = outerScope.TryGetInstance<IBar>();
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

            using (var outerScope = container.BeginScope())
            {
                using (var innerScope = container.BeginScope())
                {
                    outerScope.GetInstance<FooWithDependency>();
                    Assert.Same(outerScope, passedFactory);
                }
            }
        }

        [Fact]
        public void ShouldPassScopeToDecoratorPredicate()
        {
            var container = CreateContainer();
            IServiceFactory passedFactory = null;
            container.Register<IFoo, Foo>();
            container.Decorate<IFoo>((factory, foo) =>
            {
                passedFactory = factory;
                return new FooDecorator(foo);
            });

            using (var scope = container.BeginScope())
            {
                var instance = scope.GetInstance<IFoo>();
                Assert.Same(scope, passedFactory);
            }

        }


        [Fact]
        public void ShouldUseInitialScopeWhenResolvingParameterizedFunc()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<FooWithFuncDependency>(new PerScopeLifetime());
            container.Register<int, IFoo>((f, a) => f.GetInstance<FooWithFuncDependency>());
            using (var outerScope = container.BeginScope())
            {
                var foo = (FooWithFuncDependency)outerScope.GetInstance<int, IFoo>(42);
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
        public void ShouldNotUpdateCurrentScopeWhenCurrentScopeIsDisabled()
        {
            var container = CreateContainer(new ContainerOptions() { EnableCurrentScope = false });
            using (container.BeginScope())
            {
                Assert.Null(container.ScopeManagerProvider.GetScopeManager(container).CurrentScope);
            }
        }

        [Fact]
        public void ShouldGetInstanceWhenCurrentScopeIsDisabled()
        {
            var container = CreateContainer(new ContainerOptions() { EnableCurrentScope = false });
            container.RegisterScoped<DisposableFoo>();
            using (var scope = container.BeginScope())
            {
                var foo = scope.GetInstance<DisposableFoo>();
            }
        }

        [Fact]
        public void ShouldThrowNotImplementedForPerRequestLifeTime()
        {
            Assert.Throws<NotImplementedException>(() => new PerRequestLifeTime().GetInstance(null, null));
        }

        [Fact]
        public void ShouldThrowNotImplementedForPerScopeLifeTime()
        {
            Assert.Throws<NotImplementedException>(() => new PerScopeLifetime().GetInstance(null, null));
        }

        [Fact]
        public void ShouldThrowNotImplementedForPerContainerLifeTime()
        {
            Assert.Throws<NotImplementedException>(() => new PerContainerLifetime().GetInstance(null, null));
        }

        [Fact]
        public void ShouldUseLifetimeEx()
        {
            var container = CreateContainer(new ContainerOptions() { EnableCurrentScope = false });
            container.Register<Foo>(new LifeTimeEx());
            using (var scope = container.BeginScope())
            {
                var foo = scope.GetInstance<Foo>();
                Assert.IsType<Foo>(foo);
            }
        }


        [Fact]
        public void ShouldReturnSameInstanceWhenInstanceIsRequestedInParallel()
        {
            var container = CreateContainer(new ContainerOptions() { EnableCurrentScope = false });
            container.RegisterScoped<FooWithSlowConstructor>();
            FooWithSlowConstructor.InstanceCount = 0;
            using (var scope = container.BeginScope())
            {
                Parallel.Invoke(
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>(),
                    () => scope.GetInstance<FooWithSlowConstructor>()
                    );
            }


            Assert.Equal(1, FooWithSlowConstructor.InstanceCount);
        }

        public class FooWithSlowConstructor
        {
            public static int InstanceCount;

            public FooWithSlowConstructor()
            {
                InstanceCount++;
            }
        }
    }

    public class LifeTimeEx : ILifetime
    {
        public object GetInstance(GetInstanceDelegate createInstance, Scope scope, object[] arguments)
        {
            return createInstance(arguments, scope);
        }

        public object GetInstance(Func<object> createInstance, Scope scope)
        {
            throw new NotImplementedException();
        }
    }
}