namespace LightInject.Tests
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Dependencies;

    using LightInject.SampleLibrary;
    using LightInject.WebApi;
    
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WebApiTests : TestBase
    {
        [TestMethod]
        public void GetService_KnownService_ReturnsInstance()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            IDependencyResolver resolver = new LightInjectWebApiDependencyResolver(container);

            var instance = resolver.GetService(typeof(IFoo));
            Assert.IsInstanceOfType(instance, typeof(Foo));            
        }

        [TestMethod]
        public void GetService_UnknownService_ReturnsNull()
        {
            var container = CreateContainer();            
            IDependencyResolver resolver = new LightInjectWebApiDependencyResolver(container);

            var instance = resolver.GetService(typeof(IFoo));
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void GetServices_MultipleServices_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>();
            container.Register<IFoo, AnotherFoo>("AnotherFoo");
            IDependencyResolver resolver = new LightInjectWebApiDependencyResolver(container);

            var instances = resolver.GetServices(typeof(IFoo));
            Assert.AreEqual(2, instances.Count());
        }

        [TestMethod]
        public void GetServices_UnknownService_ReturnsEmptyEnumerable()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            IDependencyResolver resolver = new LightInjectWebApiDependencyResolver(container);

            var instances = resolver.GetServices(typeof(IFoo));

            Assert.AreEqual(0, instances.Count());
        }


        [TestMethod]
        public void Get_UsingControllerWithDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance(new[] { "SomeValue" });
            using (var server = CreateServer(container))
            {
                var client = new HttpClient(server) { BaseAddress = new Uri("http://sample:8737") };

                HttpResponseMessage responseMessage = client.GetAsync("SampleApi/0").Result;
                var result = responseMessage.Content.ReadAsAsync<string>().Result;

                Assert.AreEqual("SomeValue", result);    
            }
        }

        [TestMethod]
        public void Get_UsingControllerActionFilter_InjectsDependencyIntoActionFilter()
        {
            SampleWebApiActionFilter.StaticValue = string.Empty;
            var container = CreateContainer();
            container.RegisterInstance(new[] { "SomeValue" });
            container.RegisterInstance("SomeValue");
            var server = CreateServer(container);

            var client = new HttpClient(server) { BaseAddress = new Uri("http://sample:8737") };

            client.GetAsync("SampleApi/0").Wait();

            Assert.AreEqual("SomeValue", SampleWebApiActionFilter.StaticValue);
        }


        private HttpServer CreateServer(IServiceContainer container)
        {
            var configuration = new HttpConfiguration() { IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always };
            container.EnableWebApi(configuration);
            container.RegisterApiControllers();

            configuration.Routes.MapHttpRoute("Default", "{controller}/{id}");            
            var server = new HttpServer(configuration);            
            return server;
        }       
    }
}