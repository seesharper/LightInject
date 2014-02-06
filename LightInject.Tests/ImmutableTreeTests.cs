namespace LightInject.Tests
{
    using LightInject.SampleLibrary;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ImmutableHashTreeTests
    {
        [TestMethod]
        public void Add_LeftRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;

            var node = root.Add(10, 10).Add(20, 20).Add(30, 30);

            AssertThatTheNodeIsBalanced(node);
        }

        [TestMethod]
        public void Add_RightRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;

            var node = root.Add(30, 30).Add(20, 20).Add(10, 10);

            AssertThatTheNodeIsBalanced(node);
        }

        private static void AssertThatTheNodeIsBalanced(ImmutableHashTree<int, int> ImmutableHashTree)
        {
            Assert.AreEqual(20, ImmutableHashTree.Key);
            Assert.AreEqual(2, ImmutableHashTree.Height);
            Assert.AreEqual(10, ImmutableHashTree.Left.Key);
            Assert.AreEqual(1, ImmutableHashTree.Left.Height);
            Assert.AreEqual(30, ImmutableHashTree.Right.Key);
            Assert.AreEqual(1, ImmutableHashTree.Right.Height);
        }

        [TestMethod]
        public void Add_LeftRightRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;

            var node = root.Add(10, 10).Add(30, 30).Add(20, 20);

            AssertThatTheNodeIsBalanced(node);
        }

        [TestMethod]
        public void Add_RightLeftRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;

            var node = root.Add(30, 30).Add(10, 10).Add(20, 20);

            AssertThatTheNodeIsBalanced(node);
        }

        [TestMethod]
        public void Add_NoRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);
            AssertThatTheNodeIsBalanced(node);
        }

        [TestMethod]
        public void Search_TwoObjectsWithSameHashCodeUsingEquals_CanGetFirstObject()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var node = root.Add(new FooWithSameHashCode(1), 10).Add(new FooWithSameHashCode(2), 20);

            var result = node.Search(new FooWithSameHashCode(1));

            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Search_TwoObjectsWithSameHashCodeUsingReferenceEquals_CanGetFirstObject()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var firstKey = new FooWithSameHashCode(1);
            var secondKey = new FooWithSameHashCode(2);
            var node = root.Add(firstKey, 10).Add(secondKey, 20);

            var result = node.Search(firstKey);

            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Search_TwoObjectsWithSameHashCodeUsingEquals_CanGetLastObject()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var node = root.Add(new FooWithSameHashCode(1), 10).Add(new FooWithSameHashCode(2), 20);

            var result = node.Search(new FooWithSameHashCode(2));

            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void Search_TwoObjectsWithSameHashCodeUsingReferenceEquals_CanGetLastObject()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var firstKey = new FooWithSameHashCode(1);
            var secondKey = new FooWithSameHashCode(2);
            var thirdKey = new FooWithSameHashCode(3);
            
            var node = root.Add(firstKey, 10).Add(secondKey, 20).Add(thirdKey, 30);

            var result = node.Search(thirdKey);

            Assert.AreEqual(30, result);
        }


        [TestMethod]
        public void Search_UnknownKeyWithSameHashcodeAsExisting_ReturnsDefaultValue()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var node = root.Add(new FooWithSameHashCode(1), 10).Add(new FooWithSameHashCode(2), 20 );

            var result = node.Search(new FooWithSameHashCode(30));

            Assert.AreEqual(0, result);
        }



        [TestMethod]
        public void Search_RootNode_ReturnsNode()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);


            int value = node.Search(20);

            Assert.AreEqual(20, value);
        }

        [TestMethod]
        public void Search_LeftNode_ReturnsNode()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);

            int value = node.Search(10);

            Assert.AreEqual(10, value);
        }

        [TestMethod]
        public void Search_RightNode_ReturnsNode()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);

            int value = node.Search(30);

            Assert.AreEqual(30, value);
        }

        [TestMethod]
        public void Search_NonExistingKey_ReturnsEmpty()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);

            var result = node.Search(40);

            Assert.AreEqual(0, result);
        }
    }
}