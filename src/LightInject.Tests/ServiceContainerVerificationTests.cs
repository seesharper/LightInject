#if NET40 || NET45 || NETSTANDARD11 || NETSTANDARD13 || NET46
using Xunit;

namespace LightInject.Tests
{
    using System;
    using System.IO;

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