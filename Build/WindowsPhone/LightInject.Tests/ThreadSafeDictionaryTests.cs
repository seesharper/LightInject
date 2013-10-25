namespace LightInject.Tests
{
    using System;
    using System.Collections;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ThreadSafeDictionaryTests
    {
        [TestMethod] 
        public void TryAdd_CanHandleMultipleThreads()
        {
            var dictionary = new ThreadSafeDictionary<string, string>();            
            ParallelInvoker.Invoke(50, Add(dictionary), Remove(dictionary));                         
        }

        [TestMethod]
        public void GetEnumerator_CastToIEnumerble_ReturnsEnumerator()
        {
            var dictionary = new ThreadSafeDictionary<string, string>();
            var enumerator = ((IEnumerable)dictionary).GetEnumerator();
            
            Assert.IsNotNull(enumerator);
        }


        private static Action Add(ThreadSafeDictionary<string, string> dictionary)
        {
            return () =>
                {
                    Thread.Sleep(50);
                    for (int i = 0; i < 50; i++)
                    {
                        dictionary.TryAdd("Key", "Value");
                    }
                    
                };
        }

        private static Action Remove(ThreadSafeDictionary<string, string> dictionary)
        {
            return () =>
            {
                Thread.Sleep(50);
                for (int i = 0; i < 50; i++)
                {
                    string value;
                    dictionary.TryRemove("Key", out value);
                }

            };
        }
    }
}