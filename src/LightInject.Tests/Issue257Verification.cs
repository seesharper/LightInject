#if USE_ASSEMBLY_VERIFICATION
namespace LightInject.Tests
{
    using Xunit;

    [Trait("Category", "Verification")]
    [Collection("Verification")]
    public class Issue257Verification : Issue257
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}
#endif