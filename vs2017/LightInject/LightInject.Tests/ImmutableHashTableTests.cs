using System;
using LightInject.SampleLibrary;
using Xunit;

namespace LightInject.Tests
{
    public class ImmutableHashTableTests
    {
        [Fact]
        public void Search_ExistingType_ReturnsValue()
        {
            ImmutableHashTable<Type, int> hashTable = ImmutableHashTable<Type, int>.Empty;
            hashTable = hashTable.Add(typeof (int), 42);

            var value = hashTable.Search(typeof (int));

            Assert.Equal(42, value);
        }

        [Fact]
        public void Search_NonExistingType_ReturnsDefaultValue()
        {
            ImmutableHashTable<Type, int> hashTable = ImmutableHashTable<Type, int>.Empty;
                        
            var value = hashTable.Search(typeof(int));

            Assert.Equal(0, value);
        }

        [Fact]
        public void Search_ExistingKey_ReturnsValue()
        {
            ImmutableHashTable<int, int> hashTable = ImmutableHashTable<int, int>.Empty;
            hashTable = hashTable.Add(1, 42);

            var value = hashTable.Search(1);

            Assert.Equal(42, value);
        }

        [Fact]
        public void Search_NonExistingKey_ReturnsValue()
        {
            ImmutableHashTable<int, int> hashTable = ImmutableHashTable<int, int>.Empty;            

            var value = hashTable.Search(1);

            Assert.Equal(0, value);
        }

        [Fact]
        public void Search_SameHashCode_ReturnsValue()
        {
            ImmutableHashTable<FooWithSameHashCode, int> hashTable = ImmutableHashTable<FooWithSameHashCode, int>.Empty;

            var firstKey = new FooWithSameHashCode(42);
            var secondKey = new FooWithSameHashCode(84);

            hashTable = hashTable.Add(firstKey, 42);
            hashTable = hashTable.Add(secondKey, 84);

            var value = hashTable.Search(firstKey);

            Assert.Equal(42, value);
        }

        [Fact]
        public void AddValues_CausingTableResize_PreservesExistingValues()
        {
            ImmutableHashTable<int, int> hashTable = ImmutableHashTable<int, int>.Empty;
            hashTable = hashTable.Add(2, 2);
            hashTable = hashTable.Add(4, 4);
            hashTable = hashTable.Add(8, 8);

            var value = hashTable.Search(2);
            Assert.Equal(2, value);
        }

        [Fact]
        public void Search_TwoObjectsWithSameHashCodeUsingEquals_CanGetFirstObject()
        {
            var root = ImmutableHashTable<FooWithSameHashCode, int>.Empty;
            var node = root.Add(new FooWithSameHashCode(1), 10).Add(new FooWithSameHashCode(2), 20);

            var result = node.Search(new FooWithSameHashCode(1));

            Assert.Equal(10, result);
        }

        [Fact]
        public void Search_TwoObjectsWithSameHashCodeUsingReferenceEquals_CanGetFirstObject()
        {
            var root = ImmutableHashTable<FooWithSameHashCode, int>.Empty;
            var firstKey = new FooWithSameHashCode(1);
            var secondKey = new FooWithSameHashCode(2);
            var node = root.Add(firstKey, 10).Add(secondKey, 20);

            var result = node.Search(firstKey);

            Assert.Equal(10, result);
        }

        [Fact]
        public void Search_TwoObjectsWithSameHashCodeUsingEquals_CanGetLastObject()
        {
            var root = ImmutableHashTable<FooWithSameHashCode, int>.Empty;
            var node = root.Add(new FooWithSameHashCode(1), 10).Add(new FooWithSameHashCode(2), 20);

            var result = node.Search(new FooWithSameHashCode(2));

            Assert.Equal(20, result);
        }

        [Fact]
        public void Search_TwoObjectsWithSameHashCodeUsingReferenceEquals_CanGetLastObject()
        {
            var root = ImmutableHashTable<FooWithSameHashCode, int>.Empty;
            var firstKey = new FooWithSameHashCode(1);
            var secondKey = new FooWithSameHashCode(2);
            var thirdKey = new FooWithSameHashCode(3);

            var node = root.Add(firstKey, 10).Add(secondKey, 20).Add(thirdKey, 30);

            var result = node.Search(thirdKey);

            Assert.Equal(30, result);
        }
    }
}