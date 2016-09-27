namespace LightInject.Tests
{
    using System;
    using System.Threading;

    using LightInject.SampleLibrary;

    using Xunit;
    

    
    public class PerThreadScopeManagerTests
    {
        //private readonly ThreadLocal<PerLogicalCallContextScopeManager> scopeManagers = new ThreadLocal<PerLogicalCallContextScopeManager>(() => new PerLogicalCallContextScopeManager());

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
        public void BeginScope_WithParentScope_ParentScopeHasInnerScopeAsChild()
        {
            var container = new ServiceContainer();
            var scopeManager = new PerThreadScopeManager(container);
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
            var container = new ServiceContainer();
            var scopeManager = new PerThreadScopeManager(container);

            using (var outerScope = scopeManager.BeginScope())
            {
                using (var innerScope = scopeManager.BeginScope())
                {
                    Assert.Throws<InvalidOperationException>(() => outerScope.Dispose());
                }
            }
        }

        //[Fact]
        //public void Dispose_OnAnotherThread_ThrowsException()
        //{
        //    var container = new ServiceContainer();
        //    var scopeManager = new PerThreadScopeManager(container);
        //    Scope scope = scopeManager.BeginScope();
        //    Thread thread = new Thread(scope.Dispose);

        //    Assert.Throws<InvalidOperationException>(() =>
        //    {
        //        thread.Start();
        //        thread.Join();
        //    });
        //}

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
            container.EndCurrentScope();

            Assert.Null(manager.CurrentScope);
        }        
    }
}