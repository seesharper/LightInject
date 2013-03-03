namespace LightInject.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstructorInjectionVerificationTests : ConstructorInjectionTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return ContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}