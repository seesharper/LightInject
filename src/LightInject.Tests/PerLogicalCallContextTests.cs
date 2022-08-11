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

        private void DoInNewScope(Action action)
        {
            using (container.BeginScope())
            {
                Task.Delay(100).Wait();
                action();
            }
        }

        private async Task DoInNewScopeAsync(Action action)
        {
            using (container.BeginScope())
            {
                await Task.Delay(100);
                action();
            }
        }

        [Fact]
        public void Will_get_different_instances_in_different_scopes_with_Task_run()
        {
            IFoo foo1 = null;
            IFoo foo2 = null;

            Task.WaitAll(
                Task.Run(() => DoInNewScope(() => foo1 = container.GetInstance<IFoo>())),
                Task.Run(() => DoInNewScope(() => foo2 = container.GetInstance<IFoo>()))
            );

            Assert.NotSame(foo1, foo2);
        }

        [Fact]
        public async Task Will_get_different_instances_in_different_scopes_with_async_await()
        {
            IFoo foo1 = null;
            IFoo foo2 = null;

            await Task.WhenAll(
                Task.Run(() => DoInNewScopeAsync(() => foo1 = container.GetInstance<IFoo>())),
                Task.Run(() => DoInNewScopeAsync(() => foo2 = container.GetInstance<IFoo>()))
            );

            Assert.NotSame(foo1, foo2);
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}