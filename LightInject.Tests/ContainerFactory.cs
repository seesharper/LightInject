using System;
using System.IO;

namespace LightInject.Tests
{
    internal static class ContainerFactory
    {
        internal static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}