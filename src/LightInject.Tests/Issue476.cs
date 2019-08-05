using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class Issue476
    {
        [Fact]
        public void ShouldHandleIssue476()
        {
            var container = new ServiceContainer();

            container.Register<IFoo>(factory => new Foo());
            container.GetInstance<IFoo>();

            var clonedContainer = container.Clone();

            clonedContainer.GetInstance<IFoo>();  // IndexOutOfRangeException
        }
    }
}