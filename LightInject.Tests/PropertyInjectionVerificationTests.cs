namespace LightInject.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PropertyInjectionVerificationTests : PropertyInjectionTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return ContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}