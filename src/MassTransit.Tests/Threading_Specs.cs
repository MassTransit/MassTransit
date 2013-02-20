namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Magnum.Extensions;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture]
    public class When_configuring_the_thread_pool_for_a_high_number_of_consumers :
        LoopbackTestFixture
    {
        ManualResetEvent _wait;
        Semaphore _before;
        Semaphore _after;

        public When_configuring_the_thread_pool_for_a_high_number_of_consumers()
        {
            _wait = new ManualResetEvent(false);
            _before = new Semaphore(0, 100);
            _after = new Semaphore(0, 100);
        }

        protected override void ConfigureLocalBus(BusConfigurators.ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.SetConcurrentConsumerLimit(100);

            configurator.Subscribe(s =>
                s.Handler<A>(msg =>
                    {
                        _before.Release();
                        _wait.WaitOne(30.Seconds());
                        _after.Release();
                    }));
        }

        class A
        {
        }

        [Test, Explicit]
        public void Should_scale_threads_to_meet_demand()
        {
            var now = DateTime.Now;
            var latency = new List<long>();

            for (int i = 0; i < 100; i++)
            {
                var timer = Stopwatch.StartNew();
                LocalBus.Publish(new A());
                Assert.IsTrue(_before.WaitOne(30.Seconds()), "Consumer thread failed to start");
                timer.Stop();
                latency.Add(timer.ElapsedMilliseconds);
            }

            Console.WriteLine("Average latency: {0}ms", latency.Average());

            _wait.Set();

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(_after.WaitOne(30.Seconds()), "Consumer thread failed to complete");
            }

            Console.WriteLine("Elapsed Time: {0}", DateTime.Now - now);
        }
    }
}
