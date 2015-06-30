namespace LightInject.Tests
{
    using System.Linq;
    using System.Text;

    using LightInject.SampleLibrary;

    using Xunit;

    
    public class ImmutableHashTreeTests
    {
        [Fact]
        public void Add_LeftRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;

            var node = root.Add(10, 10).Add(20, 20).Add(30, 30);

            AssertThatTheNodeIsBalanced(node);
        }

        [Fact]
        public void Add_RightRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;

            var node = root.Add(30, 30).Add(20, 20).Add(10, 10);

            AssertThatTheNodeIsBalanced(node);
        }

        private static void AssertThatTheNodeIsBalanced(ImmutableHashTree<int, int> ImmutableHashTree)
        {
            Assert.Equal(20, ImmutableHashTree.Key);
            Assert.Equal(2, ImmutableHashTree.Height);
            Assert.Equal(10, ImmutableHashTree.Left.Key);
            Assert.Equal(1, ImmutableHashTree.Left.Height);
            Assert.Equal(30, ImmutableHashTree.Right.Key);
            Assert.Equal(1, ImmutableHashTree.Right.Height);
        }

        [Fact]
        public void Add_LeftRightRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;

            var node = root.Add(10, 10).Add(30, 30).Add(20, 20);

            AssertThatTheNodeIsBalanced(node);
        }

        [Fact]
        public void Add_RightLeftRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;

            var node = root.Add(30, 30).Add(10, 10).Add(20, 20);

            AssertThatTheNodeIsBalanced(node);
        }

        [Fact]
        public void Add_NoRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);
            AssertThatTheNodeIsBalanced(node);
        }

        [Fact]
        public void Search_TwoObjectsWithSameHashCodeUsingEquals_CanGetFirstObject()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var node = root.Add(new FooWithSameHashCode(1), 10).Add(new FooWithSameHashCode(2), 20);

            var result = node.Search(new FooWithSameHashCode(1));

            Assert.Equal(10, result);
        }

        [Fact]
        public void Search_TwoObjectsWithSameHashCodeUsingReferenceEquals_CanGetFirstObject()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var firstKey = new FooWithSameHashCode(1);
            var secondKey = new FooWithSameHashCode(2);
            var node = root.Add(firstKey, 10).Add(secondKey, 20);

            var result = node.Search(firstKey);

            Assert.Equal(10, result);
        }

        [Fact]
        public void Search_TwoObjectsWithSameHashCodeUsingEquals_CanGetLastObject()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var node = root.Add(new FooWithSameHashCode(1), 10).Add(new FooWithSameHashCode(2), 20);

            var result = node.Search(new FooWithSameHashCode(2));

            Assert.Equal(20, result);
        }

        [Fact]
        public void Search_TwoObjectsWithSameHashCodeUsingReferenceEquals_CanGetLastObject()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var firstKey = new FooWithSameHashCode(1);
            var secondKey = new FooWithSameHashCode(2);
            var thirdKey = new FooWithSameHashCode(3);
            
            var node = root.Add(firstKey, 10).Add(secondKey, 20).Add(thirdKey, 30);

            var result = node.Search(thirdKey);

            Assert.Equal(30, result);
        }


        [Fact]
        public void Search_UnknownKeyWithSameHashcodeAsExisting_ReturnsDefaultValue()
        {
            var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
            var node = root.Add(new FooWithSameHashCode(1), 10).Add(new FooWithSameHashCode(2), 20 );

            var result = node.Search(new FooWithSameHashCode(30));

            Assert.Equal(0, result);
        }



        [Fact]
        public void Search_RootNode_ReturnsNode()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);


            int value = node.Search(20);

            Assert.Equal(20, value);
        }

        [Fact]
        public void Search_LeftNode_ReturnsNode()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);

            int value = node.Search(10);

            Assert.Equal(10, value);
        }

        [Fact]
        public void Search_RightNode_ReturnsNode()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);

            int value = node.Search(30);

            Assert.Equal(30, value);
        }

        [Fact]
        public void Search_NonExistingKey_ReturnsEmpty()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);

            var result = node.Search(40);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Search_AfterRightRotation_ReturnsNodes()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(30, 30).Add(20, 20).Add(10, 10);
            Assert.Equal(10, node.Search(10));
            Assert.Equal(20, node.Search(20));
            Assert.Equal(30, node.Search(30));
        }

        [Fact]
        public void Search_AfterLeftRotation_ReturnsNodes()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(10, 10).Add(20, 20).Add(30, 30);
            Assert.Equal(10, node.Search(10));
            Assert.Equal(20, node.Search(20));
            Assert.Equal(30, node.Search(30));
        }

        [Fact]
        public void Search_AfterRightLeftRotation_ReturnsNodes()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(30, 30).Add(10, 10).Add(20, 20);
            Assert.Equal(10, node.Search(10));
            Assert.Equal(20, node.Search(20));
            Assert.Equal(30, node.Search(30));
        }

        [Fact]
        public void Search_AfterLeftRightRotation_ReturnsNodes()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(10, 10).Add(30, 30).Add(20, 20);
            Assert.Equal(10, node.Search(10));
            Assert.Equal(20, node.Search(20));
            Assert.Equal(30, node.Search(30));
        }

        [Fact]
        public void InOrder_Ordered_ReturnsNodesInOrder()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(10, 10).Add(20, 20).Add(30, 30);
            var nodes = node.InOrder().ToArray();
            Assert.True(nodes.Select(n => n.Key).SequenceEqual(new[] { 10, 20, 30 }));
        }

        [Fact]
        public void InOrder_Unordered_ReturnsNodesInOrder()
        {
            var root = ImmutableHashTree<int, int>.Empty;
            var node = root.Add(30, 30).Add(20, 20).Add(10, 10);
            var nodes = node.InOrder().ToArray();
            Assert.True(nodes.Select(n => n.Key).SequenceEqual(new[] { 10, 20, 30 }));
        }
    }
}