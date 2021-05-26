namespace MassTransit.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    [Explicit]
    public class Creating_and_disposing_a_channel_executor
    {
        [Test]
        public async Task Should_be_low_impact()
        {
            var executor = new ChannelExecutor(1);

            await executor.DisposeAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task Should_be_performant()
        {
            long value = 0;

            var executor = new ChannelExecutor(10);

            const int limit = 10000;

            for (var i = 0; i < limit; i++)
            {
                await executor.Push(() =>
                {
                    Interlocked.Increment(ref value);

                    return Task.CompletedTask;
                });
            }

            await executor.DisposeAsync().ConfigureAwait(false);

            Assert.That(value, Is.EqualTo(10000));
        }
    }
}
