#if USE_ASSEMBLY_VERIFICATION
using Xunit;

namespace LightInject.Tests
{
    using System;
    using System.IO;

    [Trait("Category", "Verification")]
    [Collection("Verification")]
    public class ServiceContainerVerificationTests : ServiceContainerTests
    {
        internal override IServiceContainer CreateContainer()
        {
            return VerificationContainerFactory.CreateContainerForAssemblyVerification();
        }
    }
}
#endif