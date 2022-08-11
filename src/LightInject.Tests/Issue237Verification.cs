#if USE_ASSEMBLY_VERIFICATION

namespace LightInject.Tests
{
    using Xunit;

    [Trait("Category", "Verification")]
    [Collection("Verification")]
    public class Issue237Verification : Issue237
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}
#endif