#if NET45 || DNX451 || DNXCORE50 || NET46
namespace LightInject.Tests
{
    using Xunit;

    [Collection("Verification")]
    public class ConstructorInjectionVerificationTests : ConstructorInjectionTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}
#endif