namespace LightInject.Interception.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TypeArrayComparerTests
    {
        [TestMethod]         
        public void Equals_IdenticalArrays_ReturnsTrue()
        {
            var comparer = new TypeArrayComparer();
            Type[] firstArray = new[]{typeof(string)};
            Type[] secondArray = new[]{typeof(string)};
             
            Assert.IsTrue(comparer.Equals(firstArray, secondArray));
        }

        [TestMethod]
        public void Equals_UnidenticalArrays_ReturnsFalse()
        {
            var comparer = new TypeArrayComparer();
            Type[] firstArray = new[] { typeof(string) };
            Type[] secondArray = new[] { typeof(int) };

            Assert.IsFalse(comparer.Equals(firstArray, secondArray));
        }

        [TestMethod]
        public void Equals_SameTypeDifferenceSequence_ReturnsFalse()
        {
            var comparer = new TypeArrayComparer();
            Type[] firstArray = new[] { typeof(string), typeof(int) };
            Type[] secondArray = new[] { typeof(int), typeof(string) };

            Assert.IsFalse(comparer.Equals(firstArray, secondArray));
        }

        [TestMethod]
        public void Equals_SameTypeSameSequence_ReturnsTrue()
        {
            var comparer = new TypeArrayComparer();
            Type[] firstArray = new[] { typeof(string), typeof(int) };
            Type[] secondArray = new[] { typeof(string), typeof(int) };

            Assert.IsTrue(comparer.Equals(firstArray, secondArray));
        }

    }
}