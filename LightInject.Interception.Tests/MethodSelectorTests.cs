namespace LightInject.Interception.Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MethodSelectorTests
    {
        [TestMethod]
        public void Execute_InterfaceWithMethod_ReturnsInterfaceMethodsAndMethodsInheritedFromObject()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(IMethodWithNoParameters), Type.EmptyTypes);

            Assert.AreEqual(4, methods.Length);
        }

        [TestMethod]
        public void Execute_InterfaceWithProperty_DoesNotReturnGetAndSetMethods()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(IClassWithProperty), Type.EmptyTypes);

            Assert.IsTrue(!methods.Any(m => m.IsDeclaredBy<IClassWithProperty>()));
        }

        [TestMethod]
        public void Execute_InterfaceWithEvent_DoesNotReturnAddAndRemoveMethods()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(IClassWithEvent), Type.EmptyTypes);

            Assert.IsTrue(!methods.Any(m => m.IsDeclaredBy<IClassWithEvent>()));
        }

        [TestMethod]
        public void Execute_ClassWithVirtualMethod_ReturnsOnlyVirtualMethods()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(ClassWithVirtualAndNonVirtualMethod), Type.EmptyTypes);

            Assert.IsTrue(methods.All(m => m.IsVirtual));
        }

        [TestMethod]
        public void Execute_ClassWithProtectedVirtualMethod_ReturnsProtectedVirtualMethod()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(ClassWithVirtualProtectedMethod), Type.EmptyTypes);

            Assert.IsTrue(methods.Any(m => m.Name == "Execute"));
        }

        [TestMethod]
        public void Execute_ClassWithNoMethods_ReturnsInterceptableMethodsInheritedFromObject()
        {
            var methodSelector = new MethodSelector();

            var methods = methodSelector.Execute(typeof(ClassWithNoMethods), Type.EmptyTypes);

            Assert.AreEqual(3, methods.Length);
            Assert.IsTrue(methods.Any(m => m.Name == "ToString"));
            Assert.IsTrue(methods.Any(m => m.Name == "GetHashCode"));
            Assert.IsTrue(methods.Any(m => m.Name == "Equals"));
        }

    }
}