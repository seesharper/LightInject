namespace LightInject.Tests
{
    using System;
    using System.Threading;

    using LightInject.SampleLibrary;


    using Xunit;
    using System.Threading.Tasks;

#if NET452 || NET46 || NETCOREAPP2_0

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
                Assert.Throws<AggregateException>(() => instance.GetBar().Wait());
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