#if NET40 || NET45 || DNX451 || DNXCORE50 || NET46
namespace LightInject.Tests
{
    using LightInject.Annotation;

    using Xunit;

    [Collection("Verification")]
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
#endif