namespace LightInject.Tests
{
    using LightInject.SampleLibrary;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using LightInject.Mocking;

    using Moq;

    [TestClass]
    public class MockingTests
    {
        [TestMethod]
        public void StartMocking_ExistingService_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo,Foo>();
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object);

            var instance = container.GetInstance<IFoo>();

            Assert.AreSame(instance, fooMock.Object);
        }

        [TestMethod]
        public void StartMocking_NamedExistingService_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo");
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object, "SomeFoo");
            
            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.AreSame(instance, fooMock.Object);
        }

        [TestMethod]
        public void StartMocking_ExistingServiceAfterGetInstance_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.GetInstance<IFoo>();
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object);

            var instance = container.GetInstance<IFoo>();

            Assert.AreSame(instance, fooMock.Object);
        }


        [TestMethod]
        public void StartMocking_NamedExistingServiceAfterGetInstance_ReturnsMockInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo");
            container.GetInstance<IFoo>("SomeFoo");
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object, "SomeFoo");

            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.AreSame(instance, fooMock.Object);
        }

        [TestMethod]
        public void StartMocking_UnknownService_RetursMockInstance()
        {
            var container = CreateContainer();            
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object);

            var instance = container.GetInstance<IFoo>();

            Assert.AreSame(instance, fooMock.Object);
        }

        [TestMethod]
        public void StartMocking_NamedUnknownService_RetursMockInstance()
        {
            var container = CreateContainer();
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object, "SomeFoo");

            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.AreSame(instance, fooMock.Object);
        }

        [TestMethod]
        public void EndMocking_ExistingService_ReturnsOriginalInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object);
            container.EndMocking<IFoo>();

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void EndMocking_NamedExistingService_ReturnsOriginalInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo");
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object, "SomeFoo");
            container.EndMocking<IFoo>("SomeFoo");

            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void EndMocking_ExistingServiceAfterGetInstance_ReturnsOriginalInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object);
            container.GetInstance<IFoo>();
            container.EndMocking<IFoo>();

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void EndMocking_NamedExistingServiceAfterGetInstance_ReturnsOriginalInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>("SomeFoo");
            var fooMock = new Mock<IFoo>();
            container.StartMocking(() => fooMock.Object, "SomeFoo");
            container.GetInstance<IFoo>("SomeFoo");
            container.EndMocking<IFoo>("SomeFoo");

            var instance = container.GetInstance<IFoo>("SomeFoo");

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}