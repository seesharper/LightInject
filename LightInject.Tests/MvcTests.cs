namespace LightInject.Tests
{
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    using LightInject.Mvc;
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class MvcTests : TestBase
    {
        [TestMethod] 
        public void GetFilters_CustomFilter_InjectsPropertyDependencies()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            var filterProvider = new LightInjectFilterProvider(container);
            var actionDescriptor = CreateActionDescriptor();
            var controllerContext = CreateControllerContext();

            var filter = filterProvider.GetFilters(controllerContext, actionDescriptor).First();

            Assert.IsInstanceOfType(((SampleFilterAttribute)filter.Instance).Foo, typeof(Foo));
        }

        [TestMethod]
        public void GetService_KnownService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            IDependencyResolver resolver = new LightInjectMvcDependencyResolver(container);

            var instance = resolver.GetService<IFoo>();

            Assert.IsInstanceOfType(instance, typeof(Foo));
        }

        [TestMethod]
        public void GetService_UnknownService_ReturnsNull()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            IDependencyResolver resolver = new LightInjectMvcDependencyResolver(container);

            var instance = resolver.GetService<IFoo>();

            Assert.IsNull(instance);
        }

        [TestMethod]
        public void GetServices_MultipleServices_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, AnotherFoo>("AnotherFoo");
            IDependencyResolver resolver = new LightInjectMvcDependencyResolver(container);

            var instances = resolver.GetServices<IFoo>();

            Assert.AreEqual(2, instances.Count());
        }

        [TestMethod]
        public void GetServices_UnknownService_ReturnsEmptyEnumerable()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();            
            IDependencyResolver resolver = new LightInjectMvcDependencyResolver(container);

            var instances = resolver.GetServices<IFoo>();

            Assert.AreEqual(0, instances.Count());
        }

        private static ActionDescriptor CreateActionDescriptor()
        {
            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(typeof(SampleController));
            var method = typeof(SampleController).GetMethod("Execute");
            ActionDescriptor actionDescriptor = new ReflectedActionDescriptor(method, "Execute", controllerDescriptor);
            return actionDescriptor;
        }

        private static ControllerContext CreateControllerContext()
        {
            var httpContextMock = new Mock<HttpContextBase>();
            var controllerContext = new ControllerContext(httpContextMock.Object, new RouteData(), new SampleController());
            return controllerContext;
        }
    }
}