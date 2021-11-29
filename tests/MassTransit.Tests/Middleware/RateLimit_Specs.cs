namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JunkDrawer;
    using MassTransit.Middleware;
    using NUnit.Framework;


    [TestFixture]
    public class Specifying_a_rate_limit
    {
        [Test]
        [Explicit]
        public async Task Should_allow_dynamic_reconfiguration_down()
        {
            var router = new PipeRouter();
            var count = 0;
            IPipe<InputContext> pipe = Pipe.New<InputContext>(cfg =>
            {
                cfg.UseRateLimit(100, TimeSpan.FromSeconds(1), router);
                cfg.UseExecute(cxt =>
                {
                    Interlocked.Increment(ref count);
                });
            });

            await router.SetRateLimit(10);

            var context = new InputContext("Hello");

            var timer = Stopwatch.StartNew();

            Task[] tasks = Enumerable.Range(0, 101)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            timer.Stop();

            Assert.That(timer.ElapsedMilliseconds, Is.GreaterThan(9500));
        }

        [Test]
        [Explicit]
        public async Task Should_allow_dynamic_reconfiguration_up()
        {
            var router = new PipeRouter();
            var count = 0;
            IPipe<InputContext> pipe = Pipe.New<InputContext>(cfg =>
            {
                cfg.UseRateLimit(10, TimeSpan.FromSeconds(1), router);
                cfg.UseExecute(cxt =>
                {
                    Interlocked.Increment(ref count);
                });
            });

            await router.SetRateLimit(100);

            var context = new InputContext("Hello");

            var timer = Stopwatch.StartNew();

            Task[] tasks = Enumerable.Range(0, 101)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            timer.Stop();

            Assert.That(timer.ElapsedMilliseconds, Is.LessThan(2000));
        }

        [Test]
        [Explicit]
        public async Task Should_count_success_and_failure_as_same()
        {
            var count = 0;
            IPipe<InputContext> pipe = Pipe.New<InputContext>(cfg =>
            {
                cfg.UseRateLimit(10, TimeSpan.FromSeconds(1));
                cfg.UseExecute(cxt =>
                {
                    var index = Interlocked.Increment(ref count);
                    if (index % 2 == 0)
                        throw new IntentionalTestException();
                });
            });

            var context = new InputContext("Hello");

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

            Assert.That(timer.ElapsedMilliseconds, Is.GreaterThan(9500));
        }

        [Test]
        [Explicit]
        public async Task Should_only_do_n_messages_per_interval()
        {
            var count = 0;
            IPipe<InputContext> pipe = Pipe.New<InputContext>(x =>
            {
                x.UseRateLimit(10, TimeSpan.FromSeconds(1));
                x.UseExecute(cxt =>
                {
                    Interlocked.Increment(ref count);
                });
            });

            var context = new InputContext("Hello");

            var timer = Stopwatch.StartNew();

            Task[] tasks = Enumerable.Range(0, 101)
                .Select(index => Task.Run(async () => await pipe.Send(context)))
                .ToArray();

            await Task.WhenAll(tasks);

            timer.Stop();

            Assert.That(timer.ElapsedMilliseconds, Is.GreaterThan(9500));
        }
    }
}
