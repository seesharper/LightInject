namespace LightInject.Tests
{
    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Xunit;

    
    public class AnnotatedParametersTests : TestBase
    {
        [Fact]
        public void GetInstance_ClassWithNamedAnnotatedProperties_InjectsProperty()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithNamedAnnotatedDependency>();
            container.Register<IBar, Bar>("SomeBar");
            container.Register<IBar, AnotherBar>("AnotherBar");

            var instance = (FooWithNamedAnnotatedDependency)container.GetInstance<IFoo>();

            Assert.IsType(typeof(AnotherBar), instance.Bar);
        }
        
        
        internal override IServiceContainer CreateContainer()
        {
            var container = (ServiceContainer)base.CreateContainer();
            container.EnableAnnotatedConstructorInjection();
            return container;
        }
    }
}