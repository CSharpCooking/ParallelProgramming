using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Concurrent_Collections_Task_Solution
{
    internal class Program
    {
        static void Main()
        {
            // Define data volumes for testing
            int[] sizes = { 1_000_000, 10_000_000, 20_000_000, 30_000_000 };

            foreach (var size in sizes)
            {
                Console.WriteLine($"Testing with {size} elements:");
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

            // Adding elements
            for (int i = 0; i < size; i++) list.Add(i);
            stopwatch.Stop();
            Console.WriteLine($"List<T> Add: {stopwatch.ElapsedMilliseconds} ms");

            // Searching for an element
            stopwatch.Restart();
            var contains = list.Contains(size / 2);
            stopwatch.Stop();
            Console.WriteLine($"List<T> Search: {stopwatch.ElapsedMilliseconds} ms");

            // Removing an element
            stopwatch.Restart();
            list.Remove(size / 2);
            stopwatch.Stop();
            Console.WriteLine($"List<T> Remove: {stopwatch.ElapsedMilliseconds} ms");
        }

        static void TestDictionary(int size)
        {
            var dictionary = new Dictionary<int, int>();
            var stopwatch = Stopwatch.StartNew();

            // Adding key-value pairs
            for (int i = 0; i < size; i++) dictionary.Add(i, i);
            stopwatch.Stop();
            Console.WriteLine($"Dictionary Add: {stopwatch.ElapsedMilliseconds} ms");

            // Searching for a key
            stopwatch.Restart();
            var contains = dictionary.ContainsKey(size / 2);
            stopwatch.Stop();
            Console.WriteLine($"Dictionary Search: {stopwatch.ElapsedMilliseconds} ms");

            // Removing a key-value pair
            stopwatch.Restart();
            dictionary.Remove(size / 2);
            stopwatch.Stop();
            Console.WriteLine($"Dictionary Remove: {stopwatch.ElapsedMilliseconds} ms");
        }

        static void TestConcurrentBag(int size)
        {
            var bag = new ConcurrentBag<int>();
            var stopwatch = Stopwatch.StartNew();

            // Adding elements in parallel
            Parallel.For(0, size, i => bag.Add(i));
            stopwatch.Stop();
            Console.WriteLine($"ConcurrentBag<T> Add: {stopwatch.ElapsedMilliseconds} ms");

            // "Search" and "Remove" are not typical operations for ConcurrentBag,
            // so they can be skipped or custom logic can be implemented if necessary
        }

        static void TestConcurrentDictionary(int size)
        {
            var dictionary = new ConcurrentDictionary<int, int>();
            var stopwatch = Stopwatch.StartNew();

            // Adding key-value pairs in parallel
            Parallel.For(0, size, i => dictionary.TryAdd(i, i));
            stopwatch.Stop();
            Console.WriteLine($"ConcurrentDictionary Add: {stopwatch.ElapsedMilliseconds} ms");

            // Searching for a key
            stopwatch.Restart();
            var contains = dictionary.ContainsKey(size / 2);
            stopwatch.Stop();
            Console.WriteLine($"ConcurrentDictionary Search: {stopwatch.ElapsedMilliseconds} ms");

            // Removing a key-value pair
            stopwatch.Restart();
            int value;
            dictionary.TryRemove(size / 2, out value);
            stopwatch.Stop();
            Console.WriteLine($"ConcurrentDictionary Remove: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}

//On the quad-core Intel Core i5 9300H processor with 16 GBytes DDR4 RAM
//the program results are as follows.

//Testing with 1000000 elements:
//List<T> Add: 4 ms
//List<T> Search: 0 ms
//List<T> Remove: 0 ms
//Dictionary Add: 30 ms
//Dictionary Search: 0 ms
//Dictionary Remove: 0 ms
//ConcurrentBag<T> Add: 98 ms
//ConcurrentDictionary Add: 346 ms
//ConcurrentDictionary Search: 0 ms
//ConcurrentDictionary Remove: 0 ms

//Testing with 10000000 elements:
//List<T> Add: 57 ms
//List<T> Search: 9 ms
//List<T> Remove: 5 ms
//Dictionary Add: 347 ms
//Dictionary Search: 0 ms
//Dictionary Remove: 0 ms
//ConcurrentBag<T> Add: 1133 ms
//ConcurrentDictionary Add: 4527 ms
//ConcurrentDictionary Search: 0 ms
//ConcurrentDictionary Remove: 0 ms

//Testing with 20000000 elements:
//List<T> Add: 105 ms
//List<T> Search: 37 ms
//List<T> Remove: 13 ms
//Dictionary Add: 864 ms
//Dictionary Search: 0 ms
//Dictionary Remove: 0 ms
//ConcurrentBag<T> Add: 2330 ms
//ConcurrentDictionary Add: 9441 ms
//ConcurrentDictionary Search: 0 ms
//ConcurrentDictionary Remove: 0 ms

//Testing with 30000000 elements:
//List<T> Add: 140 ms
//List<T> Search: 30 ms
//List<T> Remove: 17 ms
//Dictionary Add: 1825 ms
//Dictionary Search: 0 ms
//Dictionary Remove: 0 ms
//ConcurrentBag<T> Add: 3536 ms
//ConcurrentDictionary Add: 11045 ms
//ConcurrentDictionary Search: 0 ms
//ConcurrentDictionary Remove: 0 ms

//Based on the test results presented, the following conclusions can be concluded:
//1.Performance in Adding Elements:
//   - `List < T >` and `Dictionary<TKey, TValue>` significantly outperform
//   in adding elements compared to parallel collections `ConcurrentBag<T>`
//   and `ConcurrentDictionary<TKey, TValue>`.
//   - The time required to add elements to parallel collections increases
//   significantly faster with the increase in the number of elements compared
//   to using standard collections.
//2.Performance in Searching and Removing Elements:
//   - The time spent on searching and removing elements in `List<T>` and
//   `Dictionary<TKey, TValue>` increases with the size of the collection
//   but remains relatively low.
//   - `ConcurrentDictionary<TKey, TValue>` shows good performance in searching
//   and removing elements, although these operations were not tested for
//   `ConcurrentBag<T>` as these operations are not typical for this collection.
//3. Impact of Multithreading:
//   - Parallel collections such as `ConcurrentBag<T>` and
//   `ConcurrentDictionary<TKey, TValue>` are intended for use in multithreaded
//   scenarios where multiple threads perform operations on the collection
//   simultaneously.
//   - Although multithreading increases the time required to add elements to
//   parallel collections due to additional synchronization costs, it provides
//   safe access from different threads without the need for external synchronization.
//4. Choosing a Collection Depending on the Scenario:
//   - For single - threaded applications or scenarios where multithreaded access
//   to the collection is not required, `List<T>` and `Dictionary<TKey, TValue>`
//   are more preferable options due to their high performance.
//   - In multithreaded applications where safe access from multiple threads is
//   required, parallel collections such as `ConcurrentBag<T>` and
//   `ConcurrentDictionary<TKey, TValue>` should be used despite their lower
//   performance in some operations because they provide safe and efficient
//   multithreaded access.
//Overall, the choice between standard and parallel collections should be based
//on specific requirementsand usage conditions, considering the trade-off between
//performance and the need for multithreaded access.  

