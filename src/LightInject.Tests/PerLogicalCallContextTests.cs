#if NET45 || NET46
namespace LightInject.Tests
{

    using System;
    using System.Threading.Tasks;
    using SampleLibrary;
    using Xunit;

    public class PerLogicalCallContextTests : IDisposable
    {
        private readonly ServiceContainer container;
        private readonly Scope scope;

        public PerLogicalCallContextTests()
        {
            container = new ServiceContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();

            scope = container.BeginScope();
        }

        private async Task DoInNewScope(Action action, int waitTime)
        {
            using (container.BeginScope())
            {
                await Task.Delay(waitTime);
                action();
            }
        }

        [Fact]
        public async Task Will_get_different_instances_in_different_scopes_async()
        {
            IFoo foo1 = null;
            IFoo foo2 = null;
            var first = DoInNewScope(() => foo1 = container.GetInstance<IFoo>(), 100);
            var second = DoInNewScope(() => foo2 = container.GetInstance<IFoo>(), 200);

            await first;
            await second;

            Assert.NotSame(foo1, foo2);
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
#endif