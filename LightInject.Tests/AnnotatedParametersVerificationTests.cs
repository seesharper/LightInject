namespace LightInject.Tests
{
    using LightInject.Annotation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AnnotatedParametersVerificationTests : AnnotatedParametersTests
    {
        internal override IServiceContainer CreateContainer()
        {
            var container = (ServiceContainer)VerificationContainerFactory.CreateContainerForAssemblyVerification();
            container.EnableAnnotatedConstructorInjection();
            return container;
        }
    }
}