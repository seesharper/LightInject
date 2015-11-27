namespace LightInject.Interception.Tests
{
    using System;

    using Xunit;

    [Collection("Interception")]
    public class TypeArrayComparerTests
    {
        [Fact]         
        public void Equals_IdenticalArrays_ReturnsTrue()
        {
            var comparer = new TypeArrayComparer();
            Type[] firstArray = new[]{typeof(string)};
            Type[] secondArray = new[]{typeof(string)};
             
            Assert.True(comparer.Equals(firstArray, secondArray));
        }

        [Fact]
        public void Equals_UnidenticalArrays_ReturnsFalse()
        {
            var comparer = new TypeArrayComparer();
            Type[] firstArray = new[] { typeof(string) };
            Type[] secondArray = new[] { typeof(int) };

            Assert.False(comparer.Equals(firstArray, secondArray));
        }

        [Fact]
        public void Equals_SameTypeDifferenceSequence_ReturnsFalse()
        {
            var comparer = new TypeArrayComparer();
            Type[] firstArray = new[] { typeof(string), typeof(int) };
            Type[] secondArray = new[] { typeof(int), typeof(string) };

            Assert.False(comparer.Equals(firstArray, secondArray));
        }

        [Fact]
        public void Equals_SameTypeSameSequence_ReturnsTrue()
        {
            var comparer = new TypeArrayComparer();
            Type[] firstArray = new[] { typeof(string), typeof(int) };
            Type[] secondArray = new[] { typeof(string), typeof(int) };

            Assert.True(comparer.Equals(firstArray, secondArray));
        }

    }
}