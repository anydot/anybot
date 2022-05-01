using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Anybot.Common
{
    public class Delayer : IDelayer
    {
        private readonly TimeSpan delay;
        private Task delayTask = Task.CompletedTask;

        public Delayer(TimeSpan delay)
        {
            this.delay = delay;
        }

        public async Task Delay(Func<Task> delayedFunc)
        {
            await Delay(async () =>
            {
                await delayedFunc().ConfigureAwait(false);
                return 0;
            }).ConfigureAwait(false);
        }

        public Task<T> Delay<T>(Func<Task<T>> delayedFunc)
        {
            var tcsReturn = new TaskCompletionSource<T>();
            var tcsDelayed = new TaskCompletionSource<bool>();

            Interlocked.Exchange(ref delayTask, tcsDelayed.Task).ContinueWith(async _ =>
            {
                var task = delayedFunc();
                var stopwatch = Stopwatch.StartNew();

#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    tcsReturn.SetResult(await task.ConfigureAwait(false));
                }
                catch (Exception e)
                {
                    tcsReturn.SetException(e);
                }
#pragma warning restore CA1031 // Do not catch general exception types

                var sleepTime = delay - stopwatch.Elapsed;

                if (sleepTime > TimeSpan.Zero)
                {
                    await Task.Delay(sleepTime).ConfigureAwait(false);
                }

                tcsDelayed.SetResult(true);
            }, TaskScheduler.Default);

            return tcsReturn.Task;
        }
    }
}