#if NET40 || NET45 || DNX451 || DNXCORE50 || NET46
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