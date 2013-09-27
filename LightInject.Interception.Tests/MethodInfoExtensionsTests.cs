namespace LightInject.Interception.Tests
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MethodInfoExtensionsTests
    {
        [TestMethod]
        public void IsPropertySetter_SetterMethod_ReturnsTrue()
        {
            var candidate = typeof(IClassWithProperty).GetProperty("Value").GetSetMethod();

            Assert.IsTrue(candidate.IsPropertySetter());
        }

        [TestMethod]
        public void IsPropertySetter_RegularMethod_ReturnsFalse()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.IsFalse(candidate.IsPropertySetter());
        }

        [TestMethod]
        public void IsPropertyGetter_GetterMethod_ReturnsTrue()
        {
            var candidate = typeof(IClassWithProperty).GetProperty("Value").GetGetMethod();

            Assert.IsTrue(candidate.IsPropertyGetter());
        }

        [TestMethod]
        public void IsPropertyGetter_RegularMethod_ReturnsFalse()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.IsFalse(candidate.IsPropertyGetter());
        }

        [TestMethod]
        public void GetProperty_SetterMethod_ReturnsProperty()
        {
            var candidate = typeof(IClassWithProperty).GetProperty("Value").GetSetMethod();

            Assert.IsNotNull(candidate.GetProperty());
        }

        [TestMethod]
        public void GetProperty_GetterMethod_ReturnsProperty()
        {
            var candidate = typeof(IClassWithProperty).GetProperty("Value").GetGetMethod();

            Assert.IsNotNull(candidate.GetProperty());
        }

        [TestMethod]
        public void GetProperty_NormalMethod_ReturnsNull()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.IsNull(candidate.GetProperty());
        }

        [TestMethod]
        public void IsDeclaredBy_ValidDeclaringType_ReturnsTrue()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.IsTrue(candidate.IsDeclaredBy<IMethodWithNoParameters>());
        }

        [TestMethod]
        public void IsDeclaredBy_InvalidDeclaringType_ReturnsFalse()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.IsFalse(candidate.IsDeclaredBy<object>());
        }

        [TestMethod]
        public void GetDeclaringType_ValidDeclaringType_ReturnsDeclaringType()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");
            
            Assert.AreEqual(candidate.DeclaringType, candidate.GetDeclaringType());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetDeclaringType_InvalidDeclaringType_ThrowsException()
        {
            var candidate = new DynamicMethod("", typeof(object), Type.EmptyTypes);

            candidate.GetDeclaringType();
        }
    }



}