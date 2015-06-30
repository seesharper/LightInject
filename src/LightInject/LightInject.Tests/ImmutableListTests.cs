namespace LightInject.Tests
{
    using System.Linq;

    using Xunit;

    
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