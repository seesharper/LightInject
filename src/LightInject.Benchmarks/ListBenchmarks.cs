using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using BenchmarkDotNet.Attributes;

namespace LightInject.Benchmarks
{
    [SimpleJob(launchCount: 3, warmupCount: 10, targetCount: 30)]
    public class ListInsertTests
    {
        private DisposableFoo[] items = new DisposableFoo[10];

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < 10; i++)
            {
                items[i] = new DisposableFoo();
            }
        }

        [Benchmark]
        public void UsingListOfT()
        {

            List<DisposableFoo> list = new List<DisposableFoo>();
            for (int i = 0; i < items.Length - 1; i++)
            {
                list.Add(items[i]);
            }
        }

        [Benchmark]
        public void UsingLinkedListOfT()
        {
            LinkedList<DisposableFoo> linkedList = new LinkedList<DisposableFoo>();

            for (int i = 0; i < items.Length - 1; i++)
            {
                linkedList.AddLast(items[i]);
            }
        }

        [Benchmark]
        public void UsingStorageOfT()
        {
            Storage<DisposableFoo> storage = new Storage<DisposableFoo>();
            for (int i = 0; i < items.Length - 1; i++)
            {
                storage.Add(items[i]);
            }
        }



        // [Benchmark]
        // public void UsingHashSetOfT()
        // {
        //     HashSet<Foo> hashSet = new HashSet<Foo>();
        //     for (int i = 0; i < items.Length - 1; i++)
        //     {
        //         hashSet.Add(items[i]);
        //     }
        // }

        [Benchmark]
        public void UsingImmutableHashTable()
        {
            int fooIndex = 0;
            ImmutableHashTree<int, DisposableFoo> hashTable = ImmutableHashTree<int, DisposableFoo>.Empty;

            for (int i = 0; i < items.Length - 1; i++)
            {
                Interlocked.Increment(ref fooIndex);
                Interlocked.Exchange(ref hashTable, hashTable.Add(fooIndex, items[i]));
            }
        }
    }


    public struct Node<T>
    {
        public T Value;

        public T Next;
    }


    public class FastLinkedList<T>
    {
        void Add(T value)
        {

        }
    }

    internal class Storage<T>
    {
        private ArrayPool<T> pool = ArrayPool<T>.Shared;

        public T[] Items = new T[0];

        private readonly object lockObject = new object();

        public int Add(T value)
        {
            int index = Array.IndexOf(Items, value);
            if (index == -1)
            {
                return TryAddValue(value);
            }

            return index;
        }

        private int TryAddValue(T value)
        {
            lock (lockObject)
            {
                int index = Array.IndexOf(Items, value);
                if (index == -1)
                {
                    index = AddValue(value);
                }

                return index;
            }
        }

        private int AddValue(T value)
        {
            int index = Items.Length;
            T[] snapshot = CreateSnapshot();
            snapshot[index] = value;
            Items = snapshot;
            return index;
        }

        private T[] CreateSnapshot()
        {
            var snapshot = new T[Items.Length + 1];
            Array.Copy(Items, snapshot, Items.Length);
            return snapshot;
        }
    }
}