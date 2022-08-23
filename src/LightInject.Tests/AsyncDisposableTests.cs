using System;
using System.Threading.Tasks;
using Xunit;

namespace LightInject.Tests
{
    public class AsyncDisposableTests : TestBase
    {
        [Fact]
        public async Task ShouldDisposeAsyncDisposable()
        {
            var container = CreateContainer();
            container.RegisterScoped<AsyncDisposable>();

            AsyncDisposable asyncDisposable = null;
            await using (var scope = container.BeginScope())
            {
                asyncDisposable = container.GetInstance<AsyncDisposable>();
            }

            Assert.True(asyncDisposable.DisposedAsync);
        }

        public class AsyncDisposable : IAsyncDisposable
        {
            public bool DisposedAsync { get; set; }
            public ValueTask DisposeAsync()
            {
                DisposedAsync = true;
                return ValueTask.CompletedTask;
            }
        }
    }




}