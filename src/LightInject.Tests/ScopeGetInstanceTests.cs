namespace LightInject.Tests
{
    using SampleLibrary;
    using Xunit;

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
    }
}