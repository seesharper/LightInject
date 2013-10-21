namespace LightInject.Tests
{
    using System;
    using System.IO;

    internal static class VerificationContainerFactory
    {
        internal static IServiceContainer CreateContainerForAssemblyVerification()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicAssembly.dll");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
           

            var serviceContainer = new ServiceContainer((returnType, parameterTypes) => new MethodBuilderMethodSkeleton(returnType, parameterTypes, path));
            return serviceContainer;
        }
    }
}