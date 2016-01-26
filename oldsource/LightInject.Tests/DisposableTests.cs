namespace LightInject.Tests
{
    

    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    

    [TestClass]
    public class DisposableTests
    {
        [TestMethod]
        public void Dispose_ServiceWithPerScopeLifetime_IsDisposed()
        {
            var container = CreateContainer();            
            var disposableFoo = new DisposableFoo();
            container.Register<IFoo>(factory => disposableFoo, new PerScopeLifetime());
            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
            }

            Assert.IsTrue(disposableFoo.IsDisposed);
        }

        [TestMethod]
        public void Dispose_ServiceWithPerRequestLifetime_IsDisposed()
        {
            var container = CreateContainer();
            var disposableFoo = new DisposableFoo();
            container.Register<IFoo>(factory => disposableFoo, new PerRequestLifeTime());
            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
            }

            Assert.IsTrue(disposableFoo.IsDisposed);
        }


        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}