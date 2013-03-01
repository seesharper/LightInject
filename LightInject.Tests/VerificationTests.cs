namespace LightInject.Tests
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VerificationTests : ServiceContainerTests
    {
        protected override object DoCreateContainer()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DynamicAssembly.dll");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var serviceContainer = new ServiceContainer(() => new MethodBuilderMethodSkeleton(path));
            return serviceContainer;
        } 
    }
}