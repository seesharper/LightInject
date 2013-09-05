namespace LightInject.Interception.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProxyBuilderVerificationTests : ProxyBuilderTests
    {
        internal override IProxyBuilder CreateProxyBuilder()
        {
            return new ProxyBuilder(new VerifiableTypeBuilderFactory());
        }
    }
}