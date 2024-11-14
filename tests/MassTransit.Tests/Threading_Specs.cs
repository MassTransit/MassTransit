namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_configuring_the_thread_pool_for_a_high_number_of_consumers :
        InMemoryTestFixture
    {
        [Test]
        [Explicit]
        public void Should_scale_threads_to_meet_demand()
        {
            var now = DateTime.Now;
            var latency = new List<long>();

            for (var i = 0; i < 100; i++)
            {
                var timer = Stopwatch.StartNew();
                Bus.Publish(new A());
                Assert.That(_before.WaitOne(TimeSpan.FromSeconds(30)), Is.True, "Consumer thread failed to start");
                timer.Stop();
                latency.Add(timer.ElapsedMilliseconds);
            }

            Console.WriteLine("Average latency: {0}ms", latency.Average());

            _wait.Set();

            for (var i = 0; i < 100; i++)
                Assert.That(_after.WaitOne(TimeSpan.FromSeconds(30)), Is.True, "Consumer thread failed to complete");

            Console.WriteLine("Elapsed Time: {0}", DateTime.Now - now);
        }

        readonly ManualResetEvent _wait;
        readonly Semaphore _before;
        readonly Semaphore _after;

        public When_configuring_the_thread_pool_for_a_high_number_of_consumers()
        {
            _wait = new ManualResetEvent(false);
            _before = new Semaphore(0, 100);
            _after = new Semaphore(0, 100);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _wait.Dispose();
            _before.Dispose();
            _after.Dispose();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            //            configurator.Subscribe(s =>
            //                s.Handler<A>(async msg =>
            //                    {
            //                        _before.Release();
            //                        _wait.WaitOne(TimeSpan.FromSeconds(30));
            //                        _after.Release();
            //                    }));
        }


        class A
        {
        }
    }
}
