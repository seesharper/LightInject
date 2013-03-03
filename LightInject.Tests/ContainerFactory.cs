using System;
using System.IO;

namespace LightInject.Tests
{
    internal static class ContainerFactory
    {
        internal static IServiceContainer CreateContainerForAssemblyVerification()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicAssembly.dll");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var serviceContainer = new ServiceContainer(() => new MethodBuilderMethodSkeleton(path));
            return serviceContainer;
        }

        internal static IServiceContainer CreateContainer()
        {
            return new ServiceContainer();
        }
    }
}