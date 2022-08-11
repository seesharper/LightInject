
using Xunit;

namespace LightInject.Tests
{
    public class ImmutableListTests
    {
        [Fact]
        public void Add_Item_ReturnsNewList()
        {
            ImmutableList<int> list = ImmutableList<int>.Empty;

            var newList = list.Add(42);

            Assert.NotSame(list, newList);
        }
    }
}