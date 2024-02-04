using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_Parallelism_Task_Solution
{
    internal class Program
    {
        static void Main()
        {
            //  Successful completion of the task
            Task successfulTask = Task.Run(() => Console.WriteLine("The successful task is performed"));
            var successfulTaskСontinuation = successfulTask.ContinueWithStringResult().ContinueWith(t => Console.WriteLine($"The task ended with the status: {t.Result}"));

            // Generate an exception in the task
            Task faultedTask = Task.Run(() =>
            {
                Console.WriteLine("The error task is performed");
                throw new InvalidOperationException("An error occurred");
            });
            var faultedTaskСontinuation = faultedTask.ContinueWithStringResult().ContinueWith(t => Console.WriteLine($"The task ended with the status: {t.Result}"));

            // Cancellation of the task
            var cts = new CancellationTokenSource();
            Task cancellableTask = Task.Run(() =>
            {
                Console.WriteLine("The task to be canceled is being performed");
                for (int i = 0; i < 100; i++)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    Thread.Sleep(10); // Work simulation
                }
            }, cts.Token);
            // Cancel the task after 100 msec.
            cts.CancelAfter(100);
            var cancellableTaskСontinuation = cancellableTask.ContinueWithStringResult().ContinueWith(t => Console.WriteLine($"The task ended with the status: {t.Result}"));

            Task.WaitAll(successfulTaskСontinuation, faultedTaskСontinuation, cancellableTaskСontinuation);
        }
    }
    public static class TaskExtensions
    {
        public static Task<string> ContinueWithStringResult(this Task task)
        {
            return task.ContinueWith(t =>
            {
                switch (t.Status)
                {
                    case TaskStatus.RanToCompletion:
                        return "RanToCompletion";
                    case TaskStatus.Faulted:
                        return "Faulted";
                    case TaskStatus.Canceled:
                        return "Canceled";
                    default:
                        return "Unknown";
                }
            });
        }
    }

}
