using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using ImTools;

namespace LightInject.Benchmarks
{
    [MemoryDiagnoser]
    public class HashTableBenchmarks
    {
        public void Setup()
        {
        }

        [Benchmark]
        public void UsingConcurrentDictonary()
        {
            var dictionary = new ConcurrentDictionary<int, object>();
            dictionary.TryAdd(1, new object());
            dictionary.TryAdd(2, new object());
            dictionary.TryAdd(3, new object());
            dictionary.TryAdd(4, new object());
            dictionary.TryAdd(5, new object());
        }

        [Benchmark]
        public void UsingImmutableHashTable()
        {
            var table = ImmutableHashTable<int, object>.Empty;
            table = table.Add(1, new object());
            table = table.Add(2, new object());
            table = table.Add(3, new object());
            table = table.Add(4, new object());
            table = table.Add(5, new object());
        }

        [Benchmark]
        public void UsingImmutableHashTree()
        {
            var tree = ImmutableHashTree<int, object>.Empty;
            tree = tree.Add(1, new object());
            tree = tree.Add(2, new object());
            tree = tree.Add(3, new object());
            tree = tree.Add(4, new object());
            tree = tree.Add(5, new object());
        }

        [Benchmark]
        public void UsingImmutableMapTree()
        {
            var tree = ImmutableMapTree<object>.Empty;
            tree = tree.Add(1, new object());
            tree = tree.Add(2, new object());
            tree = tree.Add(3, new object());
            tree = tree.Add(4, new object());
            tree = tree.Add(5, new object());
        }

        [Benchmark]
        public void UsingImMap()
        {
            var map = ImMap<object>.Empty;
            map = map.AddOrUpdate(1, new object());
            map = map.AddOrUpdate(2, new object());
            map = map.AddOrUpdate(3, new object());
            map = map.AddOrUpdate(4, new object());
            map = map.AddOrUpdate(5, new object());
        }
    }
}