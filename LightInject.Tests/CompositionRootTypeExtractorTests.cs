namespace LightInject.Tests
{
    using System.Linq;

    using LightInject.SampleLibraryWithCompositionRootTypeAttribute;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class CompositionRootTypeExtractorTests
    {
        [TestMethod]
        public void Execute_AssemblyWithCompositionRootAttribute_ReturnsOnlyCompositionRootsDefinedInAttribute()
        {
            var extractor = new CompositionRootTypeExtractor();
            var result = extractor.Execute(typeof(CompositionRoot).Assembly);

            Assert.AreEqual(1, result.Count());
        }    
    }
}