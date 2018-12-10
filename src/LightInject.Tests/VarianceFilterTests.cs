using System.Linq;
using Xunit;

namespace LightInject.Tests
{
    public class VarianceFilterTests : TestBase
    {

        [Fact]
        public void ShouldNotApplyVarianceIfFilterDoesNotMatch()
        {
            var container = CreateContainer(new ContainerOptions(){VarianceFilter = t => false});
            container.Register(typeof(IFoo<Bar>), typeof(FooWithBar));
            container.Register(typeof(IFoo<DerivedBar>), typeof(FooWithDerivedBar));

            var instances = container.GetAllInstances<IFoo<Bar>>();

            Assert.Equal(1, instances.Count());
        }

        [Fact]
        public void ShouldApplyVarianceIfFilterMatches()
        {
            var container = CreateContainer(new ContainerOptions(){VarianceFilter = t => true});
            container.Register(typeof(IFoo<Bar>), typeof(FooWithBar));
            container.Register(typeof(IFoo<DerivedBar>), typeof(FooWithDerivedBar));

            var instances = container.GetAllInstances<IFoo<Bar>>();

            Assert.Equal(2, instances.Count());
        }


        public interface IFoo<out T> {}

        public class FooWithBar : IFoo<Bar> {}

        public class FooWithDerivedBar : IFoo<DerivedBar> {}

        public class Bar {}

        public class DerivedBar : Bar {}
    }
}