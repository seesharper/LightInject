#if NETCOREAPP2_0

using System;
using System.Linq;
using Xunit;


namespace LightInject.Tests
{
    public class LifeSpanTests
    {
        [Fact]
        public void ShouldGetLifeSpans()
        {
            Assert.Equal(10, GetLifeSpan(typeof(PerRequestLifeTime)));
        }


        private int GetLifeSpan(Type lifetimeType)
        {
            var attribute = (LifeSpanAttribute)lifetimeType.GetCustomAttributes(typeof(LifeSpanAttribute), true).FirstOrDefault();

            if (attribute == null)
            {
                throw new InvalidOperationException($"The lifetime {lifetimeType} is not decorated with the LifeSpanAttribute");
            }

            return attribute.Value;
        }

    }
}

#endif