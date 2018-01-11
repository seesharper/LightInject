using System.Linq;
using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class OrderedTests : TestBase
    {
        [Fact]
        public void ShouldOrderServicesByName()
        {
            var container = CreateContainer();
            container.Register<IFoo, Foo1>("A");
            container.Register<IFoo, Foo2>("B");
            container.Register<IFoo, Foo3>("C");

            var instances = container.GetAllInstances<IFoo>().ToArray();
            Assert.IsType<Foo1>(instances[0]);
            Assert.IsType<Foo2>(instances[1]);
            Assert.IsType<Foo3>(instances[2]);
        }

        [Fact]
        public void ShouldOrderServicesByNameWithVarianceDisabled()
        {
            var container = CreateContainer(new ContainerOptions { EnableVariance = false });
            container.Register<IFoo, Foo1>("A");
            container.Register<IFoo, Foo2>("B");
            container.Register<IFoo, Foo3>("C");

            var instances = container.GetAllInstances<IFoo>().ToArray();
            Assert.IsType<Foo1>(instances[0]);
            Assert.IsType<Foo2>(instances[1]);
            Assert.IsType<Foo3>(instances[2]);
        }

        public class Foo1 : IFoo { }

        public class Foo2 : IFoo { }

        public class Foo3 : IFoo { }
    }


  
}