namespace LightInject.Interception.Tests
{
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContainerInterceptionTests
    {
        [TestMethod]         
        public void Intercept_Service_ReturnsProxyInstance()
        {
            var container = new ServiceContainer();
            container.Register<IFoo, Foo>();
            container.Intercept(sr => sr.ServiceType == typeof(IFoo), pd => pd.Implement(info => true, () => new SampleInterceptor()));

            var instance = container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(IProxy));
        }
    }
}