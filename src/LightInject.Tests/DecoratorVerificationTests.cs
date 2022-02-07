//TODO Check USE_EXPRESSIONS

#if !USE_EXPRESSIONS
using Xunit;

namespace LightInject.Tests
{

    [Trait("Category", "Verification")]
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