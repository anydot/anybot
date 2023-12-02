using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Anybot.Common.Tests
{
    [TestFixture]
    public class DelayerTests
    {
        [Test]
        public async Task Delay_WaitsForTheSecondTaskToFinishBeforeContinueWithTheSecondOne()
        {
            var tcs1 = new TaskCompletionSource<bool>();
            var tcs2 = new TaskCompletionSource<bool>();

            var delayer = new Delayer(TimeSpan.Zero);
            var t1 = delayer.Delay(() => tcs1.Task);
            var t2 = delayer.Delay(() => tcs2.Task);

            var set = new Task[] { t1, t2 };
            var tasks = Task.WhenAny(set);

            Assert.That(tasks.IsCompleted, Is.False);

            tcs1.SetResult(true);

            Assert.That(await tasks.ConfigureAwait(false), Is.EqualTo(t1));

            tcs2.SetResult(true);
            await t2.ConfigureAwait(false);
        }
    }
}
