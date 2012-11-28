namespace LightInject.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ResolutionScopeTests
    {
        [TestMethod] 
        public void Current_ScopeOptionRequired_ReturnsScopedInstance()
        {
            var scopeManager = new ScopeManager();
            using (scopeManager.BeginScope(ScopeOption.Required))
            {
                Assert.IsNotNull(scopeManager.Current);
            }
        }

        [TestMethod]
        public void Current_ScopeOptionSuppress_ReturnsNull()
        {
            var scopeManager = new ScopeManager();
            using (scopeManager.BeginScope(ScopeOption.Suppress))
            {
                Assert.IsNull(scopeManager.Current);
            }
        }


        [TestMethod]
        public void Current_ScopeOptionRequiredWithAmbientScope_ReturnsAmbientScopedInstance()
        {
            var scopeManager = new ScopeManager();
            using (scopeManager.BeginScope(ScopeOption.Required))
            {
                var ambientContext = scopeManager.Current;
                using (scopeManager.BeginScope(ScopeOption.Required))
                {
                    Assert.AreSame(scopeManager.Current, ambientContext);
                }
            }
        }


        [TestMethod]
        public void Current_ScopeOptionRequired_IsNullWhenScopedEnds()
        {
            var scopeManager = new ScopeManager();
            using (scopeManager.BeginScope(ScopeOption.Required))
            {                
            }

            Assert.IsNull(scopeManager.Current);
        }

        [TestMethod]
        public void Current_ScopeOptionRequiredWithAmbientSuppressScope_CreatesContext()
        {
            var scopeManager = new ScopeManager();
            using (scopeManager.BeginScope(ScopeOption.Suppress))
            {                
                using (scopeManager.BeginScope(ScopeOption.Required))
                {
                    Assert.IsNotNull(scopeManager.Current);
                }

                Assert.IsNull(scopeManager.Current);
            }
        }
    }
}