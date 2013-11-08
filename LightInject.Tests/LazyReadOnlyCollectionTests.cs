namespace LightInject.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LazyReadOnlyCollectionTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Add_ThrowsException()
        {
            new LazyReadOnlyCollection<string>(null, 0).Add("SomeValue");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Remove_ThrowsException()
        {
            new LazyReadOnlyCollection<string>(null, 0).Remove("SomeValue");
        }
        
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]         
        public void Clear_ThrowsException()
        {
             new LazyReadOnlyCollection<string>(null, 0).Clear();
        }

        [TestMethod]        
        public void Contains_KnownValue_ReturnsTrue()
        {
            var lazy = new Lazy<IEnumerable<string>>(() => new[] { "SomeValue" });
            var collection = new LazyReadOnlyCollection<string>(lazy, 1);
            
            Assert.IsTrue(collection.Contains("SomeValue"));
        }

        [TestMethod]
        public void Contains_UnknownValue_ReturnsFalse()
        {
            var lazy = new Lazy<IEnumerable<string>>(() => new[] { "SomeValue" });
            var collection = new LazyReadOnlyCollection<string>(lazy, 1);

            Assert.IsFalse(collection.Contains("AnotherValue"));
        }

        [TestMethod]
        public void GetGenericEnumerator_ReturnsEnumerator()
        {
            var lazy = new Lazy<IEnumerable<string>>(() => new[] { "SomeValue" });
            var collection = new LazyReadOnlyCollection<string>(lazy, 1);

            IEnumerator<string> enumerator = collection.GetEnumerator();

            Assert.IsNotNull(enumerator);
        }

        [TestMethod]
        public void GetNonGenericEnumerator_ReturnsEnumerator()
        {
            var lazy = new Lazy<IEnumerable<string>>(() => new[] { "SomeValue" });
            var collection = new LazyReadOnlyCollection<string>(lazy, 1);

            IEnumerator enumerator = ((IEnumerable)collection).GetEnumerator();

            Assert.IsNotNull(enumerator);
        }

        [TestMethod]
        public void ToList_CollectionWithItems_ReturnsList()
        {
            var lazy = new Lazy<IEnumerable<string>>(() => new[] { "SomeValue" });
            var collection = new LazyReadOnlyCollection<string>(lazy, 1);

            var list = collection.ToList();

            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void ToList_CollectionWithoutItems_ReturnsList()
        {
            var lazy = new Lazy<IEnumerable<string>>(() => new string[] {});
            var collection = new LazyReadOnlyCollection<string>(lazy, 0);

            var list = collection.ToList();

            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void Count_DeclaredMethod_ReturnsCount()
        {
            var lazy = new Lazy<IEnumerable<string>>(() => new[] { "SomeValue" });
            var collection = new LazyReadOnlyCollection<string>(lazy, 1);

            Assert.AreEqual(1, collection.Count);
        }

        [TestMethod]
        public void Count_EnumerableExtensionMethod_ReturnsCount()
        {
            var lazy = new Lazy<IEnumerable<string>>(() => new[] { "SomeValue" });
            var collection = new LazyReadOnlyCollection<string>(lazy, 1);

            Assert.AreEqual(1, collection.Count());
        }

        [TestMethod]
        public void IsReadOnly_ReturnsTrue()
        {
            Assert.IsTrue(new LazyReadOnlyCollection<string>(null, 0).IsReadOnly);
        }
    }
}