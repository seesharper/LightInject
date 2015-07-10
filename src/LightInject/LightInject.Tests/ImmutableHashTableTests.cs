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

        
    }
}