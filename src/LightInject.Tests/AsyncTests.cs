using System;
using System.Threading;
using System.Threading.Tasks;
using LightInject.SampleLibrary;
using Xunit;
namespace LightInject.Tests
{

#if NET452 || NET46 || NETCOREAPP3_1

    public class AsyncTests : TestBase
    {
        [Fact]
        public void GetInstance_Continuation_ThrowException()
        {
            var container = new ServiceContainer();
            container.ScopeManagerProvider = new PerThreadScopeManagerProvider();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IAsyncFoo, AsyncFoo>();

            using (container.BeginScope())
            {
                var instance = container.GetInstance<IAsyncFoo>();
                // This no longer throws since we injected lazy is closed around the scope.
                // Assert.Throws<AggregateException>(() => instance.GetBar().Wait());
                var bar = instance.GetBar().Result;
                Assert.NotNull(bar);
            }
        }

        [Fact]
        public void GetInstance_Continuation_ReturnsInstance()
        {
            var container = new ServiceContainer();
            container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IAsyncFoo, AsyncFoo>();

            using (container.BeginScope())
            {
                var instance = container.GetInstance<IAsyncFoo>();
                var bar = instance.GetBar().Result;
                Assert.IsAssignableFrom<IBar>(bar);
            }
        }

        [Fact]
        public void GetInstance_SameExecutionContext_InstancesAreSame()
        {
            var container = new ServiceContainer();
            container.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IAsyncFoo, AsyncFoo>();

            using (container.BeginScope())
            {
                var firstBar = container.GetInstance<IBar>();
                var instance = container.GetInstance<IAsyncFoo>();
                var secondBar = instance.GetBar().Result;
                Assert.Same(firstBar, secondBar);
            }
        }
    }
#endif
}