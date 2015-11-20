#if NET40 || NET45 || DNX451 || DNXCORE50 || NET46
namespace LightInject.Tests
{
    using System;
    using System.IO;
    using System.Reflection;

    internal static class VerificationContainerFactory
    {
        internal static IServiceContainer CreateContainerForAssemblyVerification()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicAssembly.dll");
            if (File.Exists(path))
            {
                File.Delete(path);
            }


            var container = new ServiceContainer();

            Func<Type, Type[], IMethodSkeleton> methodSkeletonFactory =
                (returnType, parameterTypes) => new MethodBuilderMethodSkeleton(returnType, parameterTypes, path);

            var factoryField = typeof(ServiceContainer).GetField("methodSkeletonFactory", BindingFlags.Instance | BindingFlags.NonPublic);

            factoryField.SetValue(container, methodSkeletonFactory);

            //var serviceContainer = new ServiceContainer((returnType, parameterTypes) => new MethodBuilderMethodSkeleton(returnType, parameterTypes, path));
            return container;
        }
    }
}
#endif