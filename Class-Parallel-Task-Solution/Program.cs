using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace Class_Parallel_Task_Solution
{
    internal class Program
    {
        static void Main()
        {
            string wordLookupFile = Path.Combine(Path.GetTempPath(), "WordLookup.txt");

            if (!File.Exists(wordLookupFile)) // Contains about 150000 words
                new WebClient().DownloadFile(
                  "https://csharpcooking.github.io/data/allwords.txt", wordLookupFile);

            var wordLookup = new HashSet<string>(
              File.ReadAllLines(wordLookupFile),
              StringComparer.InvariantCultureIgnoreCase);

            var random = new Random();
            string[] wordList = wordLookup.ToArray();

            string[] wordsToTest = Enumerable.Range(0, 1_000_000)
              .Select(i => wordList[random.Next(0, wordList.Length)])
              .ToArray();

            // Introducing a few spelling mistakes
            wordsToTest[340] = "woozsh";
            wordsToTest[2_342] = "wubsie";
            wordsToTest[32_344] = "adgdgr";
            wordsToTest[502_348] = "dfgsie";

            int errorCount = 0; // A variable to track the number of errors
            var misspellings = new ConcurrentBag<Tuple<int, string>>();

            ParallelLoopResult result = Parallel.ForEach(wordsToTest, (word, state, i) =>
            {
                if (!wordLookup.Contains(word))
                {
                    // Increasing the error counter and checking whether the maximum value has been reached
                    if (Interlocked.Increment(ref errorCount) == 4)
                    {
                        // Break provides the guarantee that all iterations with smaller indices
                        // than the one on which Break was called will be processed.
                        state.Break(); // Early termination of loops using the Break method

                        // Calling Stop instead of Break causes forced termination of all threads
                        // immediately after their current iteration.
                        // state.Stop(); // Early termination of loops using the Stop method
                    };
                    misspellings.Add(Tuple.Create((int)i, word));
                }
            });

            foreach (var misspelling in misspellings)
            {
                Console.WriteLine($"Misspelled word '{misspelling.Item2}' found at position {misspelling.Item1}");
            }

            // Checking if the Break was called
            if (result.LowestBreakIteration.HasValue)
            {
                Console.WriteLine($"Break called at iteration {result.LowestBreakIteration.Value}");
            }
            else
            {
                Console.WriteLine("Break was not called.");
            }
        }
    }
}
