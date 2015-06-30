using System.Reflection;
using LightMock;

namespace LightInject.Tests
{
    public class CompositionRootAttributeExtractorMock : MockContext<ICompositionRootAttributeExtractor> ,ICompositionRootAttributeExtractor
    {
        public CompositionRootTypeAttribute[] GetAttributes(Assembly assembly)
        {
            return ((IInvocationContext<ICompositionRootAttributeExtractor>) this).Invoke(c => c.GetAttributes(assembly));
        }
    }
}