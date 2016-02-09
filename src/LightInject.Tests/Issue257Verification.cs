#if NET45 || DNX451 || DNXCORE50 || NET46
namespace LightInject.Tests
{
    using Xunit;

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