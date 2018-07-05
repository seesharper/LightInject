using System;
using System.Linq;
using System.Reflection;
using LightMock;
using Xunit;

namespace LightInject.Tests
{
    public class Issue373
    {
        public abstract class BaseInnerFoo
        {
        }

        public class FooWithInnerFoo
        {
            public class InnerFoo : BaseInnerFoo { }
        }

        public class BarWithInnerFoo
        {
            public class InnerFoo : BaseInnerFoo { }
        }

        [Fact]
        public void RegisterAssembly_can_register_multiple_inner_types_with_Same_name()
        {
            var container = new ServiceContainer();
            var compositionRootExtractorMock = new TypeExtractorMock();
            compositionRootExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(Type.EmptyTypes);
            container.CompositionRootTypeExtractor = compositionRootExtractorMock;
            container.RegisterAssembly(typeof(Issue373).GetTypeInfo().Assembly, (s, i) => s == typeof(BaseInnerFoo));

            Assert.True(container.AvailableServices.Any(sr => sr.ImplementingType == typeof(FooWithInnerFoo.InnerFoo)));
            Assert.True(container.AvailableServices.Any(sr => sr.ImplementingType == typeof(BarWithInnerFoo.InnerFoo)));
        }
    }
}