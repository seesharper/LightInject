#if USE_ASYNCDISPOSABLE
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LightInject.Tests
{
    public class AsyncDisposableTests : TestBase
    {
        [Fact]
        public async Task ShouldCallCompletedHandlerOnDisposeAsync()
        {
            var container = CreateContainer();
            List<object> disposedObjects = new();

            container.RegisterScoped<AsyncDisposable>(sf => new AsyncDisposable(disposedObject => disposedObjects.Add(disposedObject)));

            AsyncDisposable asyncDisposable = null;
            bool hasCompleted = false;
            await using (var scope = container.BeginScope())
            {
                scope.Completed += (o, e) => hasCompleted = true;
                asyncDisposable = container.GetInstance<AsyncDisposable>();
            }

            Assert.Contains(asyncDisposable, disposedObjects);
            Assert.True(hasCompleted);
        }
        
        [Fact]
        public async Task ShouldDisposeAsyncDisposable()
        {
            var container = CreateContainer();
            List<object> disposedObjects = new();

            container.RegisterScoped<AsyncDisposable>(sf => new AsyncDisposable(disposedObject => disposedObjects.Add(disposedObject)));

            AsyncDisposable asyncDisposable = null;

            await using (var scope = container.BeginScope())
            {
                asyncDisposable = container.GetInstance<AsyncDisposable>();
            }

            Assert.Contains(asyncDisposable, disposedObjects);
        }

        [Fact]
        public async Task ShouldDisposeSlowAsyncDisposable()
        {
            var container = CreateContainer();
            List<object> disposedObjects = new();
            container.RegisterScoped<SlowAsyncDisposable>(sf => new SlowAsyncDisposable(disposedObject => disposedObjects.Add(disposedObject)));

            SlowAsyncDisposable asyncDisposable = null;
            await using (var scope = container.BeginScope())
            {
                asyncDisposable = container.GetInstance<SlowAsyncDisposable>();
            }

            Assert.Contains(asyncDisposable, disposedObjects);
        }

        [Fact]
        public async Task ShouldDisposeInCorrectOrder()
        {
            var container = CreateContainer();
            List<object> disposedObjects = new();
            container.RegisterScoped<AsyncDisposable>(sf => new AsyncDisposable(disposedObject => disposedObjects.Add(disposedObject)));
            container.RegisterScoped<SlowAsyncDisposable>(sf => new SlowAsyncDisposable(disposedObject => disposedObjects.Add(disposedObject)));
            container.RegisterScoped<Disposable>(sf => new Disposable(disposedObject => disposedObjects.Add(disposedObject)));

            AsyncDisposable asyncDisposable = null;
            SlowAsyncDisposable slowAsyncDisposable = null;
            Disposable disposable = null;
            await using (var scope = container.BeginScope())
            {
                disposable = container.GetInstance<Disposable>();
                asyncDisposable = container.GetInstance<AsyncDisposable>();
                slowAsyncDisposable = container.GetInstance<SlowAsyncDisposable>();
            }

            Assert.Same(disposedObjects[0], slowAsyncDisposable);
            Assert.Same(disposedObjects[1], asyncDisposable);
            Assert.Same(disposedObjects[2], disposable);
        }

        [Fact]
        public async Task ShouldDisposeDisposable()
        {
            var container = CreateContainer();
            List<object> disposedObjects = new();

            container.RegisterScoped<Disposable>(sf => new Disposable(disposedObject => disposedObjects.Add(disposedObject)));
            Disposable disposable = null;
            await using (var scope = container.BeginScope())
            {
                disposable = container.GetInstance<Disposable>();
            }

            Assert.Contains(disposable, disposedObjects);
        }

        [Fact]
        public void ShouldThrowWhenAsyncDisposableIsDisposedInSynchronousScope()
        {
            var container = CreateContainer();
            container.RegisterScoped<AsyncDisposable>(sf => new AsyncDisposable(_ => { }));

            AsyncDisposable asyncDisposable = null;
            var scope = container.BeginScope();
            asyncDisposable = container.GetInstance<AsyncDisposable>();

            Assert.Throws<InvalidOperationException>(() => scope.Dispose());
        }

        [Fact]
        public async Task DisposeAsync_DuplicateAsyncDisposable_IsDisposedOnce()
        {
            var container = CreateContainer();
            var disposeCount = 0;
            var instance = new AsyncDisposable(_ => disposeCount++);

            await using (var scope = container.BeginScope())
            {
                scope.TrackInstance(instance);
                scope.TrackInstance(instance);
            }

            Assert.Equal(1, disposeCount);
        }

        [Fact]
        public async Task DisposeAsync_DuplicateDisposable_IsDisposedOnce()
        {
            var container = CreateContainer();
            var disposeCount = 0;
            var instance = new Disposable(_ => disposeCount++);

            await using (var scope = container.BeginScope())
            {
                scope.TrackInstance(instance);
                scope.TrackInstance(instance);
            }

            Assert.Equal(1, disposeCount);
        }

        [Fact]
        public async Task DisposeAsync_DuplicateAsyncDisposableWithSlowDisposable_IsDisposedOnce()
        {
            var container = CreateContainer();
            var disposeCount = 0;
            var instance = new AsyncDisposable(_ => disposeCount++);

            await using (var scope = container.BeginScope())
            {
                // instance at index 0 and 1; SlowAsyncDisposable at index 2.
                // DisposeAsync processes highest index first: SlowAsyncDisposable triggers
                // the Await path, which then processes index 1 (disposes instance) and
                // index 0 (duplicate — hits the continue branch in Await).
                scope.TrackInstance(instance);
                scope.TrackInstance(instance);
                scope.TrackInstance(new SlowAsyncDisposable(_ => { }));
            }

            Assert.Equal(1, disposeCount);
        }

        public class SlowAsyncDisposable : IAsyncDisposable
        {
            private readonly Action<object> onDisposed;

            public SlowAsyncDisposable(Action<object> onDisposed)
            {
                this.onDisposed = onDisposed;
            }
            public async ValueTask DisposeAsync()
            {
                await Task.Delay(100);
                onDisposed(this);
            }
        }

        public class AsyncDisposable : IAsyncDisposable
        {
            private readonly Action<object> onDisposed;

            public AsyncDisposable(Action<object> onDisposed)
            {
                this.onDisposed = onDisposed;
            }
            public ValueTask DisposeAsync()
            {
                onDisposed(this);
                return ValueTask.CompletedTask;
            }
        }

        public class Disposable : IDisposable
        {
            private readonly Action<object> onDisposed;

            public Disposable(Action<object> onDisposed)
            {
                this.onDisposed = onDisposed;
            }

            public void Dispose()
            {
                onDisposed(this);
            }
        }
    }
}
#endif