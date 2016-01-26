namespace LightInject.Tests
{
    using System;
    using System.Linq;

    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AnnotatedPropertiesTests : TestBase
    {        
        [TestMethod]
        public void GetInstance_ClassWithoutAnnotatedProperties_DoesNotInjectProperty()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithProperyDependency>();
            container.Register<IBar, Bar>();

            var instance = (FooWithProperyDependency)container.GetInstance<IFoo>();

            Assert.IsNull(instance.Bar);
        }

        [TestMethod]
        public void GetInstance_ClassWithAnnotatedProperties_InjectsProperty()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithAnnotatedProperyDependency>();
            container.Register<IBar, Bar>();

            var instance = (FooWithAnnotatedProperyDependency)container.GetInstance<IFoo>();

            Assert.IsNotNull(instance.Bar);
        }

        [TestMethod]
        public void GetInstance_ClassWithAnnotatedProperties_ThrowsExceptionWhenDependencyIsMissing()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithNamedAnnotatedProperyDependency>();

            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());
        }

        [TestMethod]
        public void GetInstance_ClassWithNamedAnnotatedProperties_InjectsProperty()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithNamedAnnotatedProperyDependency>();
            container.Register<IBar, Bar>("SomeBar");
            container.Register<IBar, AnotherBar>("AnotherBar");

            var instance = (FooWithNamedAnnotatedProperyDependency)container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance.Bar, typeof(AnotherBar));
        }


        internal override IServiceContainer CreateContainer()
        {
            var container = (ServiceContainer)base.CreateContainer();
            container.EnableAnnotatedPropertyInjection();            
            return container;
        }
    }
}