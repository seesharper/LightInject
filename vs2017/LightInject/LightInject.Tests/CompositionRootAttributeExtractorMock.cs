using System.Reflection;
using LightMock;

namespace LightInject.Tests
{
    internal class CompositionRootAttributeExtractorMock : MockContext<ICompositionRootAttributeExtractor> ,ICompositionRootAttributeExtractor
    {
        public CompositionRootTypeAttribute[] GetAttributes(Assembly assembly)
        {
            return ((IInvocationContext<ICompositionRootAttributeExtractor>) this).Invoke(c => c.GetAttributes(assembly));
        }
    }
}