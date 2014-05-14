namespace LightInject.Tests
{
    using System;
    using System.Threading;

    using LightInject.SampleLibrary;


    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    [TestClass]
    public class AsyncTests : TestBase
    {
        [TestMethod]        
        public void GetInstance_Continuation_ThrowException()
        {
            var container = new ServiceContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IAsyncFoo, AsyncFoo>();

            using (container.BeginScope())
            {
                var instance = container.GetInstance<IAsyncFoo>();
                ExceptionAssert.Throws<AggregateException>(() => instance.GetBar().Wait());                
            }
        }

        [TestMethod]
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
                Assert.IsInstanceOfType(bar, typeof(IBar));
            }
        }

        [TestMethod]
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
                Assert.AreSame(firstBar, secondBar);
            }
        }
    }
}