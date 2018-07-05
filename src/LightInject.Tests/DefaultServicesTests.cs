using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class DefaultServicesTests : TestBase
    {
        [Fact]
        public void ShouldBeAbleToSpecifyDefaultService()
        {
            var container = CreateContainer(new ContainerOptions() { DefaultServiceSelector = names => "AnotherFoo" });
            container.Register<IFoo, Foo>("SomeFoo");
            container.Register<IFoo, AnotherFoo>("AnotherFoo");

            var defaultInstance = container.GetInstance<IFoo>();

            Assert.IsType<AnotherFoo>(defaultInstance);
        }

        [Fact]
        public void ShouldPickCustomDefaultServiceWhenInjected()
        {
            var container = CreateContainer(new ContainerOptions() { DefaultServiceSelector = names => "AnotherBar" });
            container.Register<IBar, Bar>("SomeBar");
            container.Register<IBar, AnotherBar>("AnotherBar");
            container.Register<IFoo, FooWithDependency>();

            var instance = (FooWithDependency)container.GetInstance<IFoo>();

            var defaultInstance = instance.Bar;

            Assert.IsType<AnotherBar>(defaultInstance);

        }
    }
}
