namespace LightInject.Tests
{
    using System;
    using System.Linq;

    using LightInject.Annotation;
    using LightInject.SampleLibrary;

    using Xunit;

    
    public class AnnotatedPropertiesTests : TestBase
    {        
        [Fact]
        public void GetInstance_ClassWithoutAnnotatedProperties_DoesNotInjectProperty()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithProperyDependency>();
            container.Register<IBar, Bar>();

            var instance = (FooWithProperyDependency)container.GetInstance<IFoo>();

            Assert.Null(instance.Bar);
        }

        [Fact]
        public void GetInstance_ClassWithAnnotatedProperties_InjectsProperty()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithAnnotatedProperyDependency>();
            container.Register<IBar, Bar>();

            var instance = (FooWithAnnotatedProperyDependency)container.GetInstance<IFoo>();

            Assert.NotNull(instance.Bar);
        }

        [Fact]
        public void GetInstance_ClassWithAnnotatedProperties_ThrowsExceptionWhenDependencyIsMissing()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithNamedAnnotatedProperyDependency>();

            ExceptionAssert.Throws<InvalidOperationException>(() => container.GetInstance<IFoo>());
        }

        [Fact]
        public void GetInstance_ClassWithNamedAnnotatedProperties_InjectsProperty()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithNamedAnnotatedProperyDependency>();
            container.Register<IBar, Bar>("SomeBar");
            container.Register<IBar, AnotherBar>("AnotherBar");

            var instance = (FooWithNamedAnnotatedProperyDependency)container.GetInstance<IFoo>();

            Assert.IsType(typeof(AnotherBar), instance.Bar);
        }


        internal override IServiceContainer CreateContainer()
        {
            var container = (ServiceContainer)base.CreateContainer();
            container.EnableAnnotatedPropertyInjection();            
            return container;
        }
    }
}