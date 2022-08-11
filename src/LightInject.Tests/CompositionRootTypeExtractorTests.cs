using System.Reflection;
using Xunit;
namespace LightInject.Tests
{
    public class CompositionRootTypeExtractorTests
    {
        [Fact]
        public void Execute_AssemblyWithCompositionRootAttribute_ReturnsOnlyCompositionRootsDefinedInAttribute()
        {
            var compositionRootTypeExtractorMock = new CompositionRootAttributeExtractorMock();

            var result = compositionRootTypeExtractorMock.GetAttributes(typeof(CompositionRootTypeExtractorTests).GetTypeInfo().Assembly);

            //var extractor = new CompositionRootTypeExtractor(new CompositionRootAttributeExtractor());
            //var result = extractor.Execute(typeof(CompositionRoot).Assembly);

            //Assert.Equal(1, result.Count());
        }
    }
}