using System;
using System.Threading.Tasks;

namespace Anybot.Common
{
    public interface IDelayer
    {
        Task<T> Delay<T>(Func<Task<T>> delayedFunc);

        Task Delay(Func<Task> delayedFunc);
    }
}