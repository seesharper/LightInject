namespace LightInject.Tests
{
    

    using LightInject.SampleLibrary;
    using Xunit;

    

    
    public class DisposableTests
    {
        [Fact]
        public void Dispose_ServiceWithPerScopeLifetime_IsDisposed()
        {
            var container = CreateContainer();            
            var disposableFoo = new DisposableFoo();
            container.Register<IFoo>(factory => disposableFoo, new PerScopeLifetime());
            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
            }

            Assert.True(disposableFoo.IsDisposed);
        }

        [Fact]
        public void Dispose_ServiceWithPerRequestLifetime_IsDisposed()
        {
            var container = CreateContainer();
            var disposableFoo = new DisposableFoo();
            container.Register<IFoo>(factory => disposableFoo, new PerRequestLifeTime());
            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
            }

            Assert.True(disposableFoo.IsDisposed);
        }


        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}