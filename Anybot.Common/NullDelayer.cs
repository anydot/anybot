using System;
using System.Threading.Tasks;

namespace Anybot.Common
{
    public class NullDelayer : IDelayer
    {
        public async Task<T> Delay<T>(Func<Task<T>> delayedFunc)
        {
            return await delayedFunc().ConfigureAwait(false);
        }

        public async Task Delay(Func<Task> delayedFunc)
        {
            await delayedFunc().ConfigureAwait(false);
        }
    }
}