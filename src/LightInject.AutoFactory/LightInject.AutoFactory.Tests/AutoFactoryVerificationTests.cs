using System.Reflection;
using Xunit;

namespace LightInject.AutoFactory.Tests
{
    [Collection("Verification")]
    public class AutoFactoryVerificationTests : AutoFactoryTests
    {
        protected override AutoFactoryBuilder CreateFactoryBuilder()
        {
            var factoryBuilder = new AutoFactoryBuilder(new ServiceNameResolver());
            var field = typeof(AutoFactoryBuilder).GetField(
                "typeBuilderFactory", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(factoryBuilder, new VerifiableTypeBuilderFactory());
            return factoryBuilder;
        }
    }
}