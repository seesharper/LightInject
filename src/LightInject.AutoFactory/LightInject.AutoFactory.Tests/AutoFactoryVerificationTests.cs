using System.Reflection;
using Xunit;

namespace LightInject.AutoFactory.Tests
{
    [Collection("Verification")]
    public class AutoFactoryVerificationTests : AutoFactoryTests
    {
        protected override FactoryBuilder CreateFactoryBuilder()
        {
            var factoryBuilder = new FactoryBuilder();
            var field = typeof(FactoryBuilder).GetField(
                "typeBuilderFactory", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(factoryBuilder, new VerifiableTypeBuilderFactory());
            return factoryBuilder;
        }
    }
}