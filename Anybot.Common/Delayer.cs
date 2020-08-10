using System;
using System.Collections.Concurrent;
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

                try
                {
                    tcsReturn.SetResult(await task.ConfigureAwait(false));
                }
                catch (Exception e)
                {
                    tcsReturn.SetException(e);
                }

                var sleepTime = delay - stopwatch.Elapsed;

                if (sleepTime > TimeSpan.Zero)
                {
                    await Task.Delay(sleepTime).ConfigureAwait(false);
                }

                tcsDelayed.SetResult(true);
            });

            return tcsReturn.Task;
        }
    }
}