namespace LightInject.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ImmutableImmutableHashTreeTests
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
        public void Add_DuplicateKey()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(10, 10).Add(10, 10);
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