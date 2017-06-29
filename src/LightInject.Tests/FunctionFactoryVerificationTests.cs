#if NET452 || NETSTANDARD11 || NETSTANDARD13 || NET46
namespace LightInject.Tests
{
    using Xunit;

    [Trait("Category", "Verification")]
    [Collection("Verification")]
    public class FunctionFactoryVerificationTests : FunctionFactoryTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}
#endif