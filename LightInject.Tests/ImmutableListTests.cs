namespace LightInject.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ImmutableListTests
    {
        [TestMethod]
        public void Add_Item_ReturnsNewList()
        {
            ImmutableList<int> list = ImmutableList<int>.Empty;

            var newList = list.Add(42);

            Assert.AreNotSame(list, newList);
        }

        
    }
}