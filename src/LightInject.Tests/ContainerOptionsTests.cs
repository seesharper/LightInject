using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class ContainerOptionsTest
    {
        [Fact]
        public void ShouldConfigureOptionsUsingConfigureAction()
        {
            var container = new ServiceContainer(o => o.EnablePropertyInjection = false);
            container.Register<IBar, Bar>();
            container.Register<FooWithPropertyDependency>();

            var foo = container.GetInstance<FooWithPropertyDependency>();
            Assert.Null(foo.Bar);
        }

        [Fact]
        public void ShouldRespectDefaultOptionsWhenNoOptionsArePassed()
        {
            ContainerOptions.Default.EnablePropertyInjection = false;
            var container = new ServiceContainer();
            container.Register<IBar, Bar>();
            container.Register<FooWithPropertyDependency>();

            var foo = container.GetInstance<FooWithPropertyDependency>();
            Assert.Null(foo.Bar);
            ContainerOptions.Default.EnablePropertyInjection = true;
        }
    }
}