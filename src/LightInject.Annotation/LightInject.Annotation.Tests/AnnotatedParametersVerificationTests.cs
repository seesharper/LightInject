#if NET40 || NET45 || DNX451 || DNXCORE50 || NET46
namespace LightInject.Tests
{
    using LightInject.Annotation;

    using Xunit;

    [Collection("Verification")]
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
#endif