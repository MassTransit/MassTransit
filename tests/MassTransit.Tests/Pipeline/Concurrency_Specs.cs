namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Specifying_a_concurrency_limit
    {
        [Test]
        public async Task Should_allow_just_enough_threads_at_once()
        {
            var currentCount = 0;
            var maxCount = 0;

            IPipe<ConsumeContext<Running_two_in_memory_transports.A>> pipe = Pipe.New<ConsumeContext<Running_two_in_memory_transports.A>>(x =>
            {
                x.UseConcurrencyLimit(32);
                x.UseExecuteAsync(async payload =>
                {
                    var current = Interlocked.Increment(ref currentCount);
                    while (current > maxCount)
                        Interlocked.CompareExchange(ref maxCount, current, maxCount);

                    await Task.Delay(10);

                    Interlocked.Decrement(ref currentCount);
                });
            });

            var context = new TestConsumeContext<Running_two_in_memory_transports.A>(new Running_two_in_memory_transports.A());

            Task[] tasks = Enumerable.Range(0, 500)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            maxCount.ShouldBe(32);
        }

        [Test]
        public async Task Should_prevent_too_many_threads_at_one_time()
        {
            var currentCount = 0;
            var maxCount = 0;

            IPipe<ConsumeContext<Running_two_in_memory_transports.A>> pipe = Pipe.New<ConsumeContext<Running_two_in_memory_transports.A>>(x =>
            {
                x.UseConcurrencyLimit(1);
                x.UseExecuteAsync(async payload =>
                {
                    var current = Interlocked.Increment(ref currentCount);
                    while (current > maxCount)
                        Interlocked.CompareExchange(ref maxCount, current, maxCount);

                    await Task.Delay(10);

                    Interlocked.Decrement(ref currentCount);
                });
            });

            var context = new TestConsumeContext<Running_two_in_memory_transports.A>(new Running_two_in_memory_transports.A());

            Task[] tasks = Enumerable.Range(0, 50)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            maxCount.ShouldBe(1);
        }
    }


    [TestFixture]
    public class Specifying_a_rate_limit
    {
        [Test]
        [Explicit]
        public async Task Should_count_success_and_failure_as_same()
        {
            var count = 0;
            IPipe<ConsumeContext<Running_two_in_memory_transports.A>> pipe = Pipe.New<ConsumeContext<Running_two_in_memory_transports.A>>(x =>
            {
                x.UseRateLimit(10, TimeSpan.FromSeconds(1));
                x.UseExecute(payload =>
                {
                    var index = Interlocked.Increment(ref count);
                    if (index % 2 == 0)
                        throw new IntentionalTestException();
                });
            });

            var context = new TestConsumeContext<Running_two_in_memory_transports.A>(new Running_two_in_memory_transports.A());

            var timer = Stopwatch.StartNew();

            Task[] tasks = Enumerable.Range(0, 101)
                .Select(index => Task.Run(async () =>
                {
                    try
                    {
                        await pipe.Send(context);
                    }
                    catch (Exception)
                    {
                    }
                }))
                .ToArray();

            await Task.WhenAll(tasks);

            timer.Stop();

            timer.ElapsedMilliseconds.ShouldBeGreaterThan(9500);
        }

        [Test]
        [Explicit]
        public async Task Should_only_do_n_messages_per_interval()
        {
            var count = 0;
            IPipe<ConsumeContext<Running_two_in_memory_transports.A>> pipe = Pipe.New<ConsumeContext<Running_two_in_memory_transports.A>>(x =>
            {
                x.UseRateLimit(10, TimeSpan.FromSeconds(1));
                x.UseExecute(payload =>
                {
                    Interlocked.Increment(ref count);
                });
            });

            var context = new TestConsumeContext<Running_two_in_memory_transports.A>(new Running_two_in_memory_transports.A());

            var timer = Stopwatch.StartNew();

            Task[] tasks = Enumerable.Range(0, 101)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            timer.Stop();

            timer.ElapsedMilliseconds.ShouldBeGreaterThan(9500);
        }
    }
}
