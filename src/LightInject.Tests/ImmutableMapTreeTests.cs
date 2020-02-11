namespace LightInject.Tests
{
    using System;
    using System.Collections.Generic;
    using LightInject.SampleLibrary;
    using Xunit;


    public class ImmutableHashMapTreeTests
    {
        [Fact]
        public void Add_LeftRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableMapTree<int>.Empty;

            var node = root.Add(10, 10).Add(20, 20).Add(30, 30);

            AssertThatTheNodeIsBalanced(node);
        }

        [Fact]
        public void Add_ExistingKey_ReturnsUpdatedTree()
        {
            var root = ImmutableMapTree<int>.Empty;
            var node = root.Add(10, 10).Add(20, 20).Add(30, 30);

            node = node.Add(10, 100);
            Assert.Equal(100, node.Left.Value);
        }

        [Fact]
        public void Add_RightRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableMapTree<int>.Empty;

            var node = root.Add(30, 30).Add(20, 20).Add(10, 10);

            AssertThatTheNodeIsBalanced(node);
        }

        private static void AssertThatTheNodeIsBalanced(ImmutableMapTree<int> ImmutableHashTree)
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
            var root = ImmutableMapTree<int>.Empty;

            var node = root.Add(10, 10).Add(30, 30).Add(20, 20);

            AssertThatTheNodeIsBalanced(node);
        }

        [Fact]
        public void Add_RightLeftRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableMapTree<int>.Empty;

            var node = root.Add(30, 30).Add(10, 10).Add(20, 20);

            AssertThatTheNodeIsBalanced(node);
        }

        [Fact]
        public void Add_NoRotation_ReturnsBalancedImmutableHashTree()
        {
            var root = ImmutableMapTree<int>.Empty;
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
            var node = root.Add(new FooWithSameHashCode(1), 10).Add(new FooWithSameHashCode(2), 20);

            var result = node.Search(new FooWithSameHashCode(30));

            Assert.Equal(0, result);
        }



        [Fact]
        public void Search_RootNode_ReturnsNode()
        {
            var root = ImmutableMapTree<int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);


            int value = node.Search(20);

            Assert.Equal(20, value);
        }

        [Fact]
        public void Search_LeftNode_ReturnsNode()
        {
            var root = ImmutableMapTree<int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);

            int value = node.Search(10);

            Assert.Equal(10, value);
        }

        [Fact]
        public void Search_RightNode_ReturnsNode()
        {
            var root = ImmutableMapTree<int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);

            int value = node.Search(30);

            Assert.Equal(30, value);
        }

        [Fact]
        public void Search_NonExistingKey_ReturnsEmpty()
        {
            var root = ImmutableMapTree<int>.Empty;
            var node = root.Add(20, 20).Add(10, 10).Add(30, 30);

            var result = node.Search(40);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Search_AfterRightRotation_ReturnsNodes()
        {
            var root = ImmutableMapTree<int>.Empty;
            var node = root.Add(30, 30).Add(20, 20).Add(10, 10);
            Assert.Equal(10, node.Search(10));
            Assert.Equal(20, node.Search(20));
            Assert.Equal(30, node.Search(30));
        }

        [Fact]
        public void Search_AfterLeftRotation_ReturnsNodes()
        {
            var root = ImmutableMapTree<int>.Empty;
            var node = root.Add(10, 10).Add(20, 20).Add(30, 30);
            Assert.Equal(10, node.Search(10));
            Assert.Equal(20, node.Search(20));
            Assert.Equal(30, node.Search(30));
        }

        [Fact]
        public void Search_AfterRightLeftRotation_ReturnsNodes()
        {
            var root = ImmutableMapTree<int>.Empty;
            var node = root.Add(30, 30).Add(10, 10).Add(20, 20);
            Assert.Equal(10, node.Search(10));
            Assert.Equal(20, node.Search(20));
            Assert.Equal(30, node.Search(30));
        }

        [Fact]
        public void Search_AfterLeftRightRotation_ReturnsNodes()
        {
            var root = ImmutableMapTree<int>.Empty;
            var node = root.Add(10, 10).Add(30, 30).Add(20, 20);
            Assert.Equal(10, node.Search(10));
            Assert.Equal(20, node.Search(20));
            Assert.Equal(30, node.Search(30));
        }


        /// <summary>
        /// The following tests are copied from ImTools just to make sure we have even more tests around this.
        /// </summary>
        [Fact]
        public void Test_balance_when_adding_10_items_to_the_right()
        {
            var t = ImmutableMapTree<int>.Empty;
            for (var i = 1; i <= 10; i++)
                t = t.Add(i, i);

            // 1     =>   2     =>    2     =>    2      =>       4       =>        4        =>        4           =>         4           =>         4           =>          4
            //    2     1   3       1   3      1     4        2       5        2         6        2         6            2         6            2         6            2           8
            //                            4        3   5    1   3       6    1   3     5   7   1     3   5     7      1     3   5     8      1     3   5     8      1     3     6     9
            //                                                                                                   8                  7   9                  7   9              5   7     10
            Assert.Equal(4, t.Key);
            Assert.Equal(2, t.Left.Key);
            Assert.Equal(1, t.Left.Left.Key);
            Assert.Equal(3, t.Left.Right.Key);
            Assert.Equal(8, t.Right.Key);
            Assert.Equal(6, t.Right.Left.Key);
            Assert.Equal(5, t.Right.Left.Left.Key);
            Assert.Equal(7, t.Right.Left.Right.Key);
            Assert.Equal(9, t.Right.Right.Key);
            Assert.Equal(10, t.Right.Right.Right.Key);
        }

        [Fact]
        public void Test_balance_when_adding_10_items_to_the_right_with_double_rotation()
        {
            var t = ImmutableMapTree<int>.Empty;
            t = t.Add(1, 1);
            t = t.Add(3, 3);
            t = t.Add(2, 2);
            t = t.Add(5, 5);
            t = t.Add(4, 4);
            t = t.Add(7, 7);
            t = t.Add(6, 6);
            t = t.Add(8, 8);
            t = t.Add(9, 9);
            t = t.Add(10, 10);

            // 1     =>   2     =>    2     =>    2      =>       4       =>        4        =>        4           =>         4           =>         4           =>          4
            //    2     1   3       1   3      1     4        2       5        2         6        2         6            2         6            2         6            2           8
            //                            4        3   5    1   3       6    1   3     5   7   1     3   5     7      1     3   5     8      1     3   5     8      1     3     6     9
            //                                                                                                   8                  7   9                  7   9              5   7     10
            Assert.Equal(4, t.Key);
            Assert.Equal(2, t.Left.Key);
            Assert.Equal(1, t.Left.Left.Key);
            Assert.Equal(3, t.Left.Right.Key);
            Assert.Equal(8, t.Right.Key);
            Assert.Equal(6, t.Right.Left.Key);
            Assert.Equal(5, t.Right.Left.Left.Key);
            Assert.Equal(7, t.Right.Left.Right.Key);
            Assert.Equal(9, t.Right.Right.Key);
            Assert.Equal(10, t.Right.Right.Right.Key);
        }


        [Fact]
        public void Test_balance_when_adding_10_items_to_the_left()
        {
            var t = ImmutableMapTree<int>.Empty;
            for (var i = 10; i >= 1; i--)
                t = t.Add(i, i);

            // 10  =>   10     =>   9     =>    9     =>      9      =>       7      =>        7      =>          7      =>         7      =>         7
            //        9           8   10      8   10      7      10       6       9        5       9          5       9         5       9         3       9
            //                               7          6   8           5       8   10   4   6   8   10     4   6   8   10    3   6   8   10    2   5   8   10
            //                                                                                            3                  2 4               1   4 6
            Assert.Equal(7, t.Key);
            Assert.Equal(3, t.Left.Key);
            Assert.Equal(2, t.Left.Left.Key);
            Assert.Equal(1, t.Left.Left.Left.Key);
            Assert.Equal(5, t.Left.Right.Key);
            Assert.Equal(4, t.Left.Right.Left.Key);
            Assert.Equal(6, t.Left.Right.Right.Key);
            Assert.Equal(9, t.Right.Key);
            Assert.Equal(8, t.Right.Left.Key);
            Assert.Equal(10, t.Right.Right.Key);
        }

        [Fact]
        public void Test_balance_when_adding_10_items_to_the_left_with_double_rotation()
        {
            var t = ImmutableMapTree<int>.Empty;
            t = t.Add(10, 10);
            t = t.Add(8, 8);
            t = t.Add(9, 9);
            t = t.Add(6, 6);
            t = t.Add(7, 7);
            t = t.Add(4, 4);
            t = t.Add(5, 5);
            t = t.Add(2, 2);
            t = t.Add(3, 3);
            t = t.Add(1, 1);

            // 10  =>   10     =>   9     =>    9     =>      9      =>       7      =>        7      =>          7      =>         7      =>         7
            //        9           8   10      8   10      7      10       6       9        5       9          5       9         5       9         3       9
            //                               7          6   8           5       8   10   4   6   8   10     4   6   8   10    3   6   8   10    2   5   8   10
            //                                                                                            3                  2 4               1   4 6
            Assert.Equal(7, t.Key);
            Assert.Equal(3, t.Left.Key);
            Assert.Equal(2, t.Left.Left.Key);
            Assert.Equal(1, t.Left.Left.Left.Key);
            Assert.Equal(5, t.Left.Right.Key);
            Assert.Equal(4, t.Left.Right.Left.Key);
            Assert.Equal(6, t.Left.Right.Right.Key);
            Assert.Equal(9, t.Right.Key);
            Assert.Equal(8, t.Right.Left.Key);
            Assert.Equal(10, t.Right.Right.Key);
        }

        [Fact]
        public void Test_that_all_added_values_are_accessible()
        {
            var t = ImmutableMapTree<int>.Empty
                .Add(1, 11)
                .Add(2, 22)
                .Add(3, 33);

            Assert.Equal(11, t.Search(1));
            Assert.Equal(22, t.Search(2));
            Assert.Equal(33, t.Search(3));
        }


        [Fact]
        public void Test_balance_ensured_for_left_left_tree()
        {
            var t = ImmutableMapTree<int>.Empty
                .Add(5, 1)
                .Add(4, 2)
                .Add(3, 3);

            //     5   =>    4
            //   4         3   5
            // 3
            Assert.Equal(4, t.Key);
            Assert.Equal(3, t.Left.Key);
            Assert.Equal(5, t.Right.Key);
        }

        [Fact]
        public void Test_balance_preserved_when_add_to_balanced_tree()
        {
            var t = ImmutableMapTree<int>.Empty
                .Add(5, 1)
                .Add(4, 2)
                .Add(3, 3)
                // add to that
                .Add(2, 4)
                .Add(1, 5);

            //       4    =>     4
            //     3   5      2     5
            //   2          1   3
            // 1
            Assert.Equal(4, t.Key);
            Assert.Equal(2, t.Left.Key);
            Assert.Equal(1, t.Left.Left.Key);
            Assert.Equal(3, t.Left.Right.Key);
            Assert.Equal(5, t.Right.Key);

            // parent node balancing
            t = t.Add(-1, 6);

            //         4                 2
            //      2     5   =>      1     4
            //    1   3            -1     3   5
            // -1

            Assert.Equal(2, t.Key);
            Assert.Equal(1, t.Left.Key);
            Assert.Equal(-1, t.Left.Left.Key);

            Assert.Equal(4, t.Right.Key);
            Assert.Equal(3, t.Right.Left.Key);
            Assert.Equal(5, t.Right.Right.Key);
        }

        [Fact]
        public void Test_balance_ensured_for_left_right_tree()
        {
            var t = ImmutableMapTree<int>.Empty
                .Add(5, 1)
                .Add(3, 2)
                .Add(4, 3);

            //     5  =>    5   =>   4
            //  3         4        3   5
            //    4     3
            Assert.Equal(4, t.Key);
            Assert.Equal(3, t.Left.Key);
            Assert.Equal(5, t.Right.Key);
        }

        [Fact]
        public void Test_balance_ensured_for_right_right_tree()
        {
            var t = ImmutableMapTree<int>.Empty
                .Add(3, 1)
                .Add(4, 2)
                .Add(5, 3);

            // 3      =>     4
            //   4         3   5
            //     5
            Assert.Equal(4, t.Key);
            Assert.Equal(3, t.Left.Key);
            Assert.Equal(5, t.Right.Key);
        }

        [Fact]
        public void Test_balance_ensured_for_right_left_tree()
        {
            var t = ImmutableMapTree<int>.Empty
                .Add(3, 1)
                .Add(5, 2)
                .Add(4, 3);

            // 3      =>   3     =>    4
            //    5          4       3   5
            //  4              5
            Assert.Equal(4, t.Key);
            Assert.Equal(3, t.Left.Key);
            Assert.Equal(5, t.Right.Key);
        }

        /// <summary>
        /// https://github.com/seesharper/LightInject.Microsoft.AspNetCore.Hosting/issues/54
        /// </summary>
        [Fact]
        public void Issue54()
        {
            var map = ImmutableMapTree<int>.Empty;
            map = map.Add(37, 37);
            map = map.Add(39, 39);
            map = map.Add(17, 17);
            map = map.Add(10, 10);
            map = map.Add(13, 13);
            map = map.Add(16, 16);
            map = map.Add(14, 14);
            map = map.Add(15, 15);
            map = map.Add(46, 46);
            map = map.Add(47, 47);

            var result = map.Search(10);
        }


        [Fact]
        public void SmokeTest()
        {
            var keys = GetRandomKeys();
            var map = ImmutableMapTree<int>.Empty;

            for (int i = 0; i < keys.Length; i++)
            {
                map = map.Add(keys[i], keys[i]);
            }

            for (int i = 0; i < keys.Length; i++)
            {
                Assert.Equal(keys[i], map.Search(keys[i]));
            }

        }

        private int[] GetRandomKeys()
        {
            List<int> keys = new List<int>();
            var random = new Random();
            while (keys.Count < 100)
            {
                var key = random.Next(1, 1000);
                if (!keys.Contains(key))
                {
                    keys.Add(key);
                }
            }

            return keys.ToArray();
        }


        // [Fact]
        // public void InOrder_Ordered_ReturnsNodesInOrder()
        // {
        //     var root = ImmutableMapTree<int>.Empty;
        //     var node = root.Add(10, 10).Add(20, 20).Add(30, 30);
        //     var nodes = node.InOrder().ToArray();
        //     Assert.True(nodes.Select(n => n.Key).SequenceEqual(new[] { 10, 20, 30 }));
        // }

        // [Fact]
        // public void InOrder_Unordered_ReturnsNodesInOrder()
        // {
        //     var root = ImmutableMapTree<int>.Empty;
        //     var node = root.Add(30, 30).Add(20, 20).Add(10, 10);
        //     var nodes = node.InOrder().ToArray();
        //     Assert.True(nodes.Select(n => n.Key).SequenceEqual(new[] { 10, 20, 30 }));
        // }

        // [Fact]
        // public void InOrder_DuplicatesHashCodes_ReturnsNodes()
        // {
        //     var root = ImmutableHashTree<FooWithSameHashCode, int>.Empty;
        //     var node = root.Add(new FooWithSameHashCode(42), 42).Add(new FooWithSameHashCode(84), 84);
        //     var nodes = node.InOrder();
        //     Assert.Equal(2, nodes.Count());
        // }
    }


}