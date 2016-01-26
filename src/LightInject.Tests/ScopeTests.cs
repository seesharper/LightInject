namespace LightInject.Tests
{
    using System;
    using System.Threading;

    using LightInject.SampleLibrary;

    using Xunit;
    

    
    public class ScopeTests
    {
        private readonly ThreadLocal<ScopeManager> scopeManagers = new ThreadLocal<ScopeManager>(() => new ScopeManager());

        [Fact]
        public void BeginScope_NoParentScope_ParentScopeIsNull()
        {
            using (var scope = scopeManagers.Value.BeginScope())
            {
                Assert.Null(scope.ParentScope);
            }
        }

        [Fact]
        public void BeginScope_WithParentScope_ParentScopeIsOuterScope()
        {
            using (var outerScope = scopeManagers.Value.BeginScope())
            {
                using (var scope = scopeManagers.Value.BeginScope())
                {
                    Assert.Same(scope.ParentScope, outerScope);
                }
            }
        }

        [Fact]
        public void BeginScope_WithParentScope_ParentScopeHasInnerScopeAsChild()
        {
            using (var outerScope = scopeManagers.Value.BeginScope())
            {
                using (var scope = scopeManagers.Value.BeginScope())
                {
                    Assert.Same(scope, outerScope.ChildScope);
                }
            }
        }

        [Fact]
        public void EndScope_BeforeInnerScopeHasCompleted_ThrowsException()
        {
            using (var outerScope = scopeManagers.Value.BeginScope())
            {
                using (scopeManagers.Value.BeginScope())
                {
                    Assert.Throws<InvalidOperationException>(() => scopeManagers.Value.EndScope(outerScope));
                }
            }
        }

        [Fact]
        public void Dispose_OnAnotherThread_UpdateCurrentScope()
        {
            Scope scope = scopeManagers.Value.BeginScope();
            Thread thread = new Thread(scope.Dispose);
            thread.Start();
            thread.Join();
            Assert.Null(scopeManagers.Value.CurrentScope);
        }

        [Fact]
        public void Dispose_WithTrackedInstances_DisposesTrackedInstances()
        {
            var disposable = new DisposableFoo();
            Scope scope = scopeManagers.Value.BeginScope();
            scope.TrackInstance(disposable);
            scope.Dispose();
            Assert.True(disposable.IsDisposed);
        }

        [Fact]
        public void EndCurrentScope_InScope_EndsScope()
        {
            var container = new ServiceContainer();
            ScopeManager manager = container.ScopeManagerProvider.GetScopeManager();
            
            container.BeginScope();
            container.EndCurrentScope();

            Assert.Null(manager.CurrentScope);
        }
    }
}