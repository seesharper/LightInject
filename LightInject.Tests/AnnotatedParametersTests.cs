namespace LightInject.Tests
{
    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AnnotatedParametersTests : TestBase
    {
        [TestMethod]
        public void GetInstance_ClassWithNamedAnnotatedProperties_InjectsProperty()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithNamedAnnotatedDependency>();
            container.Register<IBar, Bar>("SomeBar");
            container.Register<IBar, AnotherBar>("AnotherBar");

            var instance = (FooWithNamedAnnotatedDependency)container.GetInstance<IFoo>();

            Assert.IsInstanceOfType(instance.Bar, typeof(AnotherBar));
        }
        
        
        internal override IServiceContainer CreateContainer()
        {
            var container = (ServiceContainer)base.CreateContainer();
            container.EnableAnnotatedConstructorInjection();
            return container;
        }
    }
}