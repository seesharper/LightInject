namespace LightInject.Interception.Tests
{
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContainerInterceptionTests
    {
        [TestMethod]         
        public void Decorate_Service_ReturnsProxyInstance()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            container.Decorate(sr => sr.ServiceType == typeof(IFoo), pd => pd.Intercept(() => new SampleInterceptor(), info => true));

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }
    }
}