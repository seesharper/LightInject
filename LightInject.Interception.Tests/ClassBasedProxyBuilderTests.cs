using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightInject.Interception.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClassBasedProxyBuilderTests
    {
        [TestMethod]
        public void GetProxyType_ClassWithVirtualMethod_ReturnsSubclass()
        {
            var proxyDefinition = new ProxyDefinition(typeof(ClassWithVirtualMethod));

            Type proxyType = CreateProxyType(proxyDefinition);

            Assert.IsTrue(proxyType.IsSubclassOf(typeof(ClassWithVirtualMethod)));

        }


        private Type CreateProxyType(ProxyDefinition proxyDefinition)
        {
            return CreateProxyBuilder().GetProxyType(proxyDefinition);
        }

        internal virtual IProxyBuilder CreateProxyBuilder()
        {
            return new ProxyBuilder();
        }
    }
}
