using System.Reflection;
using Xunit;

namespace LightInject.AutoFactory.Tests
{
    [Collection("Verification")]
    public class AutoFactoryVerificationTests : AutoFactoryTests
    {
        internal override AutoFactoryBuilder CreateFactoryBuilder()
        {
            var factoryBuilder = new AutoFactoryBuilder(new VerifiableTypeBuilderFactory(),  new ServiceNameResolver());            
            return factoryBuilder;
        }

        public static void Configure(IServiceContainer container)
        { 

        }
    }
}