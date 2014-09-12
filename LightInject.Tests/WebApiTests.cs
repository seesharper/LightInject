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
        public void GetServices_MultipleServicesFromScope_ReturnsAllInstances()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            container.Register<IFoo, AnotherFoo>("AnotherFoo", new PerScopeLifetime());
            IDependencyResolver resolver = new LightInjectWebApiDependencyResolver(container);
            using (var scope = resolver.BeginScope())
            {
                var instances = scope.GetServices(typeof(IFoo));
                Assert.AreEqual(2, instances.Count());    
            }            
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

            var client = new HttpClient(server) { BaseAddress = new Uri("http://localhost:8737") };

            client.GetAsync("SampleApi/0").Wait();

            Assert.AreEqual("SomeValue", SampleWebApiActionFilter.StaticValue);
        }

        [TestMethod]
        public void Get_UsingControllerActionFilter_InjectsFuncDependencyIntoActionFilter()
        {
            WebApiActionFilterWithFuncDependency.StaticValue = null;
            var container = CreateContainer();        
            container.Register<IFooFactory>(sf => new FooFactory(() => new Foo()));

            container.Register(factory => CreateFoo(factory));            
            var server = CreateServer(container);

            var client = new HttpClient(server) { BaseAddress = new Uri("http://localhost:8737") };

            var result = client.GetAsync("AnotherSampleApi/0").Result;

            Assert.IsNotNull(WebApiActionFilterWithFuncDependency.StaticValue);
        }


        [TestMethod]
        public void GetService_MultipleThreads_DoesNotThrowInvalidScopeException()
        {
             var container = CreateContainer();
            container.Register<IFoo, Foo>(new PerScopeLifetime());
            IDependencyResolver resolver = new LightInjectWebApiDependencyResolver(container);
            
            ParallelInvoker.Invoke(10, () => GetFooWithinScope(resolver));
        }

        private void GetFooWithinScope(IDependencyResolver resolver)
        {
            using (var scope = resolver.BeginScope())
            {
                scope.GetService(typeof(IFoo));
            }
        }


        private IFoo CreateFoo(IServiceFactory sf)
        {
            return sf.GetInstance<IFooFactory>().CreateFoo();
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