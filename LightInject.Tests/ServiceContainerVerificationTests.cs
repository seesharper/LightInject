namespace LightInject.Tests
{
    using System;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceContainerVerificationTests : ServiceContainerTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return ContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}