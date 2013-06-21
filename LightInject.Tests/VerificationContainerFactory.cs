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
            return null;

            //var serviceContainer = new ServiceContainer(() => new MethodBuilderMethodSkeleton(path));
            //return serviceContainer;
        }
    }
}