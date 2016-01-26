namespace LightInject.Tests
{
    using LightInject.Annotation;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AnnotatedPropertiesVerificationTests : AnnotatedPropertiesTests
    {
        internal override IServiceContainer CreateContainer()
        {
            var container = (ServiceContainer)VerificationContainerFactory.CreateContainerForAssemblyVerification();
            container.PropertyDependencySelector = new AnnotatedPropertyDependencySelector(new PropertySelector());
            return container;
        }
    }
}