namespace LightInject.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class DisposableTests
    {
        [TestMethod]
        public void Dispose_ServiceWithPerScopeLifetime_IsDisposed()
        {
            var container = CreateContainer();
            var disposableMock = new Mock<IDisposable>();
            container.Register(factory => disposableMock.Object, new PerScopeLifetime());
            using (container.BeginScope())
            {
                container.GetInstance<IDisposable>();
            }

            disposableMock.Verify(d => d.Dispose(), Times.Once());
        }

        [TestMethod]
        public void Dispose_ServiceWithPerRequestLifetime_IsDisposed()
        {
            var container = CreateContainer();
            var disposableMock = new Mock<IDisposable>();
            container.Register(factory => disposableMock.Object, new PerRequestLifeTime());
            using (container.BeginScope())
            {
                container.GetInstance<IDisposable>();
            }

            disposableMock.Verify(d => d.Dispose(), Times.Once());
        }


        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}