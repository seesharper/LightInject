#if USE_ASSEMBLY_VERIFICATION
using Xunit;

namespace LightInject.Tests
{

    [Trait("Category", "Verification")]
    [Collection("Verification")]
    public class DefaultValuesVerificationTests : DefaultValueTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}
#endif