namespace LightInject.Interception.Tests
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    using Xunit;

    [Collection("Interception")]
    public class MethodInfoExtensionsTests
    {
        [Fact]
        public void IsPropertySetter_SetterMethod_ReturnsTrue()
        {
            var candidate = typeof(IClassWithProperty).GetProperty("Value").GetSetMethod();

            Assert.True(candidate.IsPropertySetter());
        }

        [Fact]
        public void IsPropertySetter_RegularMethod_ReturnsFalse()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.False(candidate.IsPropertySetter());
        }

        [Fact]
        public void IsPropertyGetter_GetterMethod_ReturnsTrue()
        {
            var candidate = typeof(IClassWithProperty).GetProperty("Value").GetGetMethod();

            Assert.True(candidate.IsPropertyGetter());
        }

        [Fact]
        public void IsPropertyGetter_RegularMethod_ReturnsFalse()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.False(candidate.IsPropertyGetter());
        }

        [Fact]
        public void GetProperty_SetterMethod_ReturnsProperty()
        {
            var candidate = typeof(IClassWithProperty).GetProperty("Value").GetSetMethod();

            Assert.NotNull(candidate.GetProperty());
        }

        [Fact]
        public void GetProperty_GetterMethod_ReturnsProperty()
        {
            var candidate = typeof(IClassWithProperty).GetProperty("Value").GetGetMethod();

            Assert.NotNull(candidate.GetProperty());
        }

        [Fact]
        public void GetProperty_NormalMethod_ReturnsNull()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.Null(candidate.GetProperty());
        }

        [Fact]
        public void IsDeclaredBy_ValidDeclaringType_ReturnsTrue()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.True(candidate.IsDeclaredBy<IMethodWithNoParameters>());
        }

        [Fact]
        public void IsDeclaredBy_InvalidDeclaringType_ReturnsFalse()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");

            Assert.False(candidate.IsDeclaredBy<object>());
        }

        [Fact]
        public void GetDeclaringType_ValidDeclaringType_ReturnsDeclaringType()
        {
            var candidate = typeof(IMethodWithNoParameters).GetMethod("Execute");
            
            Assert.Equal(candidate.DeclaringType, candidate.GetDeclaringType());
        }

        [Fact]        
        public void GetDeclaringType_InvalidDeclaringType_ThrowsException()
        {
            var candidate = new DynamicMethod("", typeof(object), Type.EmptyTypes);
            Assert.Throws<ArgumentException>(() => candidate.GetDeclaringType());            
        }
    }



}