namespace LightInject.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class DisposableTests
    {
        [TestMethod]
        public void Dispose_ServiceWithPerGraphLifetime_IsDisposed()
        {
            var container = CreateContainer();
            var disposableMock = new Mock<IDisposable>();
            container.Register<IDisposable>(factory => disposableMock.Object, new PerGraphLifetime());
            using (container.BeginScope())
            {
                var instance = container.GetInstance<IDisposable>();
            }

            disposableMock.Verify(d => d.Dispose(), Times.Once());
        }


        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}