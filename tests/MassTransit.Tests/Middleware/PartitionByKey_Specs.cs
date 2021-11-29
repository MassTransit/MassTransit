namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class Partitioning_a_consumer_by_key
    {
        [Test]
        public async Task Should_use_a_partitioner_for_consistency()
        {
            TaskCompletionSource<int> completed = TaskUtil.GetTask<int>();
            var count = 0;
            IPipe<InputContext> pipe = Pipe.New<InputContext>(x =>
            {
                x.UsePartitioner(8, context => (string)context.Value);
                x.UseExecute(payload =>
                {
                    if (Interlocked.Increment(ref count) == Limit)
                        completed.TrySetResult(Limit);
                });
            });

            await Task.WhenAll(Enumerable.Range(0, Limit).Select(index => pipe.Send(new InputContext(index.ToString()))));

            await completed.Task;

            Assert.AreEqual(Limit, count);

            Console.WriteLine("Processed: {0}", count);
        }

        const int Limit = 100;
    }
}
