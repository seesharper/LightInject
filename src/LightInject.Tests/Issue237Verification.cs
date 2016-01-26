#if NET45 || DNX451 || DNXCORE50 || NET46

namespace LightInject.Tests
{
    using Xunit;

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