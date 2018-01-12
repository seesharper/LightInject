using System;
using System.Reflection;
namespace LightInject.Tests
{
    using SampleLibrary;
    using LightMock;
    using Xunit;


    public class LazyCompositionRootTests
    {
        [Fact]
        public void Initialize_CompositionRootAttribute_ReturnsType()
        {
            var attribute = new CompositionRootTypeAttribute(typeof(CompositionRootMock));
            Assert.Equal(typeof(CompositionRootMock), attribute.CompositionRootType);
        }

        [Fact]
        public void GetInstance_UnknownService_ExecutesCompositionRootInSourceAssembly()
        {
            var container = new ServiceContainer();
            var compositionRootMock = new CompositionRootMock();
            compositionRootMock.Arrange(c => c.Compose(container)).Callback<IServiceContainer>(c => c.Register<IFoo, Foo>());

            var compositionRootTypeExtractorMock = new TypeExtractorMock();
            compositionRootTypeExtractorMock.Arrange(c => c.Execute(The<Assembly>.IsAnyValue)).Returns(new[] {typeof(CompositionRootMock)});

            var assemblyScanner = new AssemblyScanner(new ConcreteTypeExtractor(), compositionRootTypeExtractorMock,
                new CompositionRootExecutor(container, t => compositionRootMock), new GenericArgumentMapper(), new ServiceNameProvider());

            container.AssemblyScanner = assemblyScanner;

            container.GetInstance<IFoo>();

            compositionRootMock.Assert(c => c.Compose(container), Invoked.Once);
        }




    }
}