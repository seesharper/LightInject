using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightInject.Nancy.Tests
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Nancy;
    using global::Nancy.Bootstrapper;
    using global::Nancy.Testing;

    [TestClass]
    public class LightInjectNancyBootstrapperTests
    {
        [TestMethod]
        public void GetEngine_ReturnsEngine()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            var engine = bootstrapper.GetEngine();
            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void GetModule_ReturnsModule()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            INancyModule module = bootstrapper.GetModule(typeof(SampleModule), new NancyContext());
            Assert.IsInstanceOfType(module, typeof(SampleModule));
        }

        [TestMethod]
        public void GetModule_TransientDependency_ReturnsModule()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            var module = (SampleModuleWithTransientDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithTransientDependency),
                new NancyContext());
            Assert.IsInstanceOfType(module.Transient, typeof(Transient));
        }

        [TestMethod]
        public void GetModule_PerRequestDependency_ReturnsModule()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            var module = (SampleModuleWithPerRequestDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithPerRequestDependency),
                new NancyContext());
            Assert.IsInstanceOfType(module.PerRequest, typeof(PerRequest));
        }

        [TestMethod]
        public void GetAllModules_ReturnsModules()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            var modules = bootstrapper.GetAllModules(new NancyContext());
            Assert.IsTrue(modules.Any());
        }

        [TestMethod]
        public void GetEngine_RegistersRequestStartup()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            var engine = bootstrapper.GetEngine();
            engine.HandleRequest(new Request("Post", "Sample", "http"));
        }

        [TestMethod]
        public void GetModule_WithPerRequestCollectionTypeDependency_ReturnsModuleWithDependencies()
        {
            var bootstrapper = CreateBootstrapper();

            var module = (SampleModuleWithPerRequestCollectionDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithPerRequestCollectionDependency),
                new NancyContext());

            Assert.AreEqual(2, module.Instances.Count());
        }

        [TestMethod]
        public void GetModule_WithPerRequestCollectionTypeDependency_ReturnsDifferentDependencies()
        {
            var bootstrapper = new DefaultNancyBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();

            //var bootstrapper = CreateBootstrapper();

            var firstModule = (SampleModuleWithPerRequestCollectionDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithPerRequestCollectionDependency),
                new NancyContext());

            var secondModule = (SampleModuleWithPerRequestCollectionDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithPerRequestCollectionDependency),
                new NancyContext());
            Assert.IsTrue(firstModule.Instances.SequenceEqual(secondModule.Instances));

            Assert.AreEqual(firstModule.Instances.First(), secondModule.Instances.First());
        }

        private static TestBootstrapper CreateBootstrapper()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            return bootstrapper;
        }
    }

    internal class TestBootstrapper : LightInjectNancyBootstrapper
    {
        private IServiceContainer container;

        internal TestBootstrapper()
        {
            container = new ServiceContainer();
        }

        public T GetInstance<T>()
        {
            return container.GetInstance<T>();
        }

        protected override IServiceContainer GetServiceContainer()
        {
            return container;
        }

        protected override void RegisterTypes(IServiceContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            base.RegisterTypes(container, typeRegistrations);
        }
    }

    

    

    

    public class SampleRequestStartup : IRequestStartup
    {
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            //throw new NotImplementedException();
        }
    }

        

    

}
