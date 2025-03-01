using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Concurrent_Collections_Task_Solution
{
    internal class Program
    {
        static void Main()
        {
            // Define data sizes for testing
            int[] sizes = { 10_000, 100_000, 1_000_000 };

            foreach (var size in sizes)
            {
                Console.WriteLine($"\nTesting with {size} elements:");
                TestList(size);
                TestDictionary(size);
                TestConcurrentBag(size);
                TestConcurrentDictionary(size);
            }
        }

        static void TestList(int size)
        {
            var list = new List<int>();
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < size; i++) list.Add(i);
            stopwatch.Stop();
            Console.WriteLine($"List<int> Addition: {stopwatch.ElapsedMilliseconds} ms.");

            stopwatch.Restart();
            for (int i = 0; i < size; i++) list.Contains(i);
            stopwatch.Stop();
            Console.WriteLine($"List<int> Search: {stopwatch.ElapsedMilliseconds} ms.");

            stopwatch.Restart();
            for (int i = 0; i < size; i++) list.Remove(i);
            stopwatch.Stop();
            Console.WriteLine($"List<int> Removal: {stopwatch.ElapsedMilliseconds} ms.");
        }

        static void TestDictionary(int size)
        {
            var dictionary = new Dictionary<int, int>();
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < size; i++) dictionary.Add(i, i);
            stopwatch.Stop();
            Console.WriteLine($"Dictionary<int,int> Addition: {stopwatch.ElapsedMilliseconds} ms.");

            stopwatch.Restart();
            Parallel.For(0, size, i => dictionary.ContainsKey(i));
            stopwatch.Stop();
            Console.WriteLine($"Dictionary<int,int> Search: {stopwatch.ElapsedMilliseconds} ms.");

            stopwatch.Restart();
            for (int i = 0; i < size; i++) dictionary.Remove(i);
            stopwatch.Stop();
            Console.WriteLine($"Dictionary<int,int> Removal: {stopwatch.ElapsedMilliseconds} ms.");
        }

        static void TestConcurrentBag(int size)
        {
            var bag = new ConcurrentBag<int>();
            var stopwatch = Stopwatch.StartNew();

            Parallel.For(0, size, i => bag.Add(i));
            stopwatch.Stop();
            Console.WriteLine($"ConcurrentBag<int> Addition: {stopwatch.ElapsedMilliseconds} ms.");

            // Search and removal are not tested, as ConcurrentBag<T> is not designed for these operations.
        }

        static void TestConcurrentDictionary(int size)
        {
            var dictionary = new ConcurrentDictionary<int, int>();
            var stopwatch = Stopwatch.StartNew();

            Parallel.For(0, size, i => dictionary.TryAdd(i, i));
            stopwatch.Stop();
            Console.WriteLine($"ConcurrentDictionary<int,int> Addition: {stopwatch.ElapsedMilliseconds} ms.");

            stopwatch.Restart();
            Parallel.For(0, size, i => dictionary.ContainsKey(i));
            stopwatch.Stop();
            Console.WriteLine($"ConcurrentDictionary<int,int> Search: {stopwatch.ElapsedMilliseconds} ms.");

            stopwatch.Restart();
            Parallel.For(0, size, i => dictionary.TryRemove(i, out _));
            stopwatch.Stop();
            Console.WriteLine($"ConcurrentDictionary<int,int> Removal: {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
