#if NET40 || NET45 || NETSTANDARD11 || NETSTANDARD13 || NET46
using Xunit;

namespace LightInject.Tests
{

    [Collection("Verification")]
    public class DecoratorVerificationTests : DecoratorTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}
#endif