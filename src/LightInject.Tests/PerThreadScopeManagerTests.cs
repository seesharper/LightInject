namespace LightInject.Tests
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using LightInject.SampleLibrary;

    using Xunit;

    public class PerThreadScopeManagerTests
    {
        [Fact]
        public void BeginScope_NoParentScope_ParentScopeIsNull()
        {
            var container = new ServiceContainer();
            var scopeManager = new PerThreadScopeManager(container);

            using (var scope = scopeManager.BeginScope())
            {
                Assert.Null(scope.ParentScope);
            }
        }

        [Fact]
        public void BeginScope_WithParentScope_ParentScopeIsOuterScope()
        {
            var container = new ServiceContainer();
            var scopeManager = new PerThreadScopeManager(container);

            using (var outerScope = scopeManager.BeginScope())
            {
                using (var scope = scopeManager.BeginScope())
                {
                    Assert.Same(scope.ParentScope, outerScope);
                }
            }
        }

        [Fact]
        public void EndScope_BeforeInnerScopeHasCompleted_DoesNotThrowException()
        {
            var container = new ServiceContainer();
            var scopeManager = new PerThreadScopeManager(container);

            using (var outerScope = scopeManager.BeginScope())
            {
                using (var innerScope = scopeManager.BeginScope())
                {
                    outerScope.Dispose();
                }
            }
        }


        [Fact]
        public void Dispose_OnAnotherThread_ShouldDisposeScope()
        {
            //Note : https://stackoverflow.com/questions/11417283/strange-weakreference-behavior-on-mono

            var container = new ServiceContainer();
            var scope = container.BeginScope();
            WeakReference scopeReference = new WeakReference(scope);

            // Dispose the scope on a different thread
            Thread disposeThread = new Thread(scope.Dispose);
            disposeThread.Start();
            disposeThread.Join();

            // We are now back on the starting thread and
            // although the scope was ended on another thread
            // the current scope on this thread should reflect that.
            var currentScope = container.ScopeManagerProvider.GetScopeManager(container).CurrentScope;
            Assert.Null(currentScope);
            Assert.True(scope.IsDisposed);
        }

        [Fact]
        public void Dispose_WithTrackedInstances_DisposesTrackedInstances()
        {
            var container = new ServiceContainer();
            var scopeManager = new PerThreadScopeManager(container);
            var disposable = new DisposableFoo();
            Scope scope = scopeManager.BeginScope();
            scope.TrackInstance(disposable);
            scope.Dispose();
            Assert.True(disposable.IsDisposed);
        }

        [Fact]
        public void EndCurrentScope_InScope_EndsScope()
        {
            var container = new ServiceContainer();
            IScopeManager manager = container.ScopeManagerProvider.GetScopeManager(container);

            container.BeginScope();
            container.ScopeManagerProvider.GetScopeManager(container).CurrentScope.Dispose();

            Assert.Null(manager.CurrentScope);
        }
    }
}