#if NET45 || NETSTANDARD11 || NETSTANDARD13 || NET46
namespace LightInject.Tests
{
    using Xunit;

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