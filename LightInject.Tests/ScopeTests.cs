namespace LightInject.Tests
{
    using System;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class ScopeTests
    {
        private readonly ThreadLocal<ScopeManager> scopeManagers = new ThreadLocal<ScopeManager>(() => new ScopeManager());

        [TestMethod]
        public void BeginScope_NoParentScope_ParentScopeIsNull()
        {
            using (var scope = scopeManagers.Value.BeginScope())
            {
                Assert.IsNull(scope.ParentScope);
            }
        }

        [TestMethod]
        public void BeginScope_WithParentScope_ParentScopeIsOuterScope()
        {
            using (var outerScope = scopeManagers.Value.BeginScope())
            {
                using (var scope = scopeManagers.Value.BeginScope())
                {
                    Assert.AreSame(scope.ParentScope, outerScope);
                }
            }
        }

        [TestMethod]
        public void BeginScope_WithParentScope_ParentScopeHasInnerScopeAsChild()
        {
            using (var outerScope = scopeManagers.Value.BeginScope())
            {
                using (var scope = scopeManagers.Value.BeginScope())
                {
                    Assert.AreSame(scope, outerScope.ChildScope);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EndScope_BeforeInnerScopeHasCompleted_ThrowsException()
        {
            using (var outerScope = scopeManagers.Value.BeginScope())
            {
                using (scopeManagers.Value.BeginScope())
                {
                    scopeManagers.Value.EndScope(outerScope);
                }
            }
        }

        [TestMethod]
        public void Dispose_OnAnotherThread_UpdateCurrentScope()
        {
            Scope scope = scopeManagers.Value.BeginScope();
            Thread thread = new Thread(scope.Dispose);
            thread.Start();
            thread.Join();
            Assert.IsNull(scopeManagers.Value.CurrentScope);
        }

        [TestMethod]
        public void Dispose_WithTrackedInstances_DisposesTrackedInstances()
        {
            Mock<IDisposable> disposable = new Mock<IDisposable>();
            Scope scope = scopeManagers.Value.BeginScope();
            scope.TrackInstance(disposable.Object);
            scope.Dispose();
            disposable.Verify(d => d.Dispose(), Times.Once());
        }

        [TestMethod]
        public void EndCurrentScope_InScope_EndsScope()
        {
            var container = new ServiceContainer();
            ScopeManager manager = container.ScopeManagerProvider.GetScopeManager();
            
            container.BeginScope();
            container.EndCurrentScope();

            Assert.IsNull(manager.CurrentScope);
        }
    }
}