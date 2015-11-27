namespace LightInject.Interception.Tests
{
    using System.Reflection;

    using Xunit;

    [Collection("Interception")]
    public class ClassBasedProxyBuilderVerificationTests : ClassBasedProxyBuilderTests
    {
        internal override IProxyBuilder CreateProxyBuilder()
        {
            var proxyBuilder = new ProxyBuilder();
            var field = typeof(ProxyBuilder).GetField(
                "typeBuilderFactory", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(proxyBuilder, new VerifiableTypeBuilderFactory());
            return proxyBuilder;
        }
    }
}