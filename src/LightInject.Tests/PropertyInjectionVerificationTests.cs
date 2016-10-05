#if NET45 || NETSTANDARD11 || NETSTANDARD13 || NET46
namespace LightInject.Tests
{
    using Xunit;

    [Trait("Category", "Verification")]
    [Collection("Verification")]
    public class PropertyInjectionVerificationTests : PropertyInjectionTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}
#endif