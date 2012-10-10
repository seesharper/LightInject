namespace LightInject.Tests
{
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DecoratorTests
    {
        [TestMethod]
        public void Decorate_Service_ReturnsDecoratedInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Decorate(typeof(IFoo), typeof(FooDecorator), si => true);
            var instance = container.GetInstance<IFoo>();
            Assert.IsInstanceOfType(instance, typeof(FooDecorator));
        }

        private static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        } 
    }


}