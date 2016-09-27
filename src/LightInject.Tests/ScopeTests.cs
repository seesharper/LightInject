namespace LightInject.Tests
{
    using System;
    using System.Threading;

    using LightInject.SampleLibrary;

    using Xunit;
    

    
    public class ScopeTests
    {
        //private readonly ThreadLocal<PerLogicalCallContextScopeManager> scopeManagers = new ThreadLocal<PerLogicalCallContextScopeManager>(() => new PerLogicalCallContextScopeManager());

        [Fact]
        public void BeginScope_NoParentScope_ParentScopeIsNull()
        {
            var scopeManager = new PerThreadScopeManager();

            using (var scope = scopeManager.BeginScope())
            {
                Assert.Null(scope.ParentScope);
            }
        }

        [Fact]
        public void BeginScope_WithParentScope_ParentScopeIsOuterScope()
        {
            var scopeManager = new PerThreadScopeManager();

            using (var outerScope = scopeManager.BeginScope())
            {
                using (var scope = scopeManager.BeginScope())
                {
                    Assert.Same(scope.ParentScope, outerScope);
                }
            }
        }

        [Fact]
        public void BeginScope_WithParentScope_ParentScopeHasInnerScopeAsChild()
        {
            var scopeManager = new PerThreadScopeManager();
            using (var outerScope = scopeManager.BeginScope())
            {
                using (var scope = scopeManager.BeginScope())
                {
                    Assert.Same(scope, outerScope.ChildScope);
                }
            }
        }

        [Fact]
        public void EndScope_BeforeInnerScopeHasCompleted_ThrowsException()
        {
            var scopeManager = new PerThreadScopeManager();

            using (var outerScope = scopeManager.BeginScope())
            {
                using (var innerScope = scopeManager.BeginScope())
                {
                    Assert.Throws<InvalidOperationException>(() => outerScope.Dispose());
                }
            }
        }

        [Fact]
        public void Dispose_OnAnotherThread_UpdateCurrentScope()
        {
            var scopeManager = new PerThreadScopeManager();
            Scope scope = scopeManager.BeginScope();
            Thread thread = new Thread(scope.Dispose);
            thread.Start();
            thread.Join();
            Assert.Null(scopeManager.CurrentScope);
        }

        [Fact]
        public void Dispose_WithTrackedInstances_DisposesTrackedInstances()
        {
            var scopeManager = new PerThreadScopeManager();
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
            IScopeManager manager = container.ScopeManagerProvider.GetScopeManager();
            
            container.BeginScope();
            container.EndCurrentScope();

            Assert.Null(manager.CurrentScope);
        }
    }
}