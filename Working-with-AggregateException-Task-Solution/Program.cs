using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Working_with_AggregateException_Task_Solution
{
    internal class Program
    {
        static void Main()
        {
            // Creating a list of tasks
            var tasks = new List<Task<int>>()
            {
                Task.Run(() => GenerateResult(1)),
                Task.Run(() => GenerateResult(2)),
                Task.Run(() => GenerateResult(3)),
                Task.Run(() => GenerateResult(4)),
                Task.Run(() => GenerateResult(5))
            };

            try
            {
                // Waiting for the results of the tasks
                var results = Task.WhenAll(tasks).Result;
            }
            catch (AggregateException ae)
            {
                // Handling exceptions
                ae.Handle(ex =>
                {
                    if (ex is InvalidOperationException)
                    {
                        Console.WriteLine("Caught InvalidOperationException.");
                        return true; // This exception is handled
                    }
                    return false; // Other exceptions are not handled here
                });
            }
            finally
            {
                // Sum the results of successful tasks
                var sum = tasks.Where(t => t.Status == TaskStatus.RanToCompletion).Sum(t => t.Result);

                Console.WriteLine($"Sum of results from successful tasks: {sum}");
            }
        }
        // Method for generating result or throwing exception
        static int GenerateResult(int number)
        {
            // If the number is even, throw an InvalidOperationException
            if (number % 2 == 0)
            {
                throw new InvalidOperationException($"Invalid operation for number: {number}");
            }
            // Otherwise, return the number multiplied by 10 as a result
            return number * 10;
        }
    }
}
