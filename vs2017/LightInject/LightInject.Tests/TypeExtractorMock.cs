using System;
using System.Reflection;
using LightMock;

namespace LightInject.Tests
{
    internal class TypeExtractorMock : MockContext<ITypeExtractor>, ITypeExtractor
    {
        public Type[] Execute(Assembly assembly)
        {
            return ((IInvocationContext<ITypeExtractor>) this).Invoke(t => t.Execute(assembly));
        }
    }
}