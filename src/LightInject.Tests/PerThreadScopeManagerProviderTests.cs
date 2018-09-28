using System.Threading.Tasks;
using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class PerThreadScopeManagerProviderTests : TestBase
    {
        [Fact]
        public async Task ShouldGetInstancePerThread()
        {
            var container = CreateContainer();
            container.ScopeManagerProvider = new PerThreadScopeManagerProvider();
            container.RegisterScoped<IFoo, Foo>();

            IFoo firstInstance = null;
            IFoo secondInstance = null;

            await Task.WhenAll(
           Task.Run(async () => firstInstance = await Resolve(container)),
           Task.Run(async () => secondInstance = await Resolve(container))
           );

            Assert.NotSame(firstInstance, secondInstance);
        }

        private async Task<IFoo> Resolve(IServiceContainer container)
        {
            await Task.Delay(100);
            using (container.BeginScope())
            {
                return container.GetInstance<IFoo>();
            }
        }
    }
}