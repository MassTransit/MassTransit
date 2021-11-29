namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using MassTransit.Testing.Implementations;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture(TypeArgs = new[] { typeof(SuccessConsumer) })]
    [TestFixture(TypeArgs = new[] { typeof(ThrowConsumer) })]
    [TestFixture(TypeArgs = new[] { typeof(RandomConsumer) })]
    public class BusActivityMonitor_Specs<TConsumer> :
        InMemoryTestFixture
        where TConsumer : class, IConsumer<PingMessage>, new()
    {
        [Test]
        [Repeat(RetryPoliciesLength)]
        public async Task Should_detect_inactivity()
        {
        #pragma warning disable 4014
            ActivityTask();
        #pragma warning restore 4014

            var timeout = await _activityMonitor.AwaitBusInactivity(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            Assert.IsTrue(timeout, "Activity monitor timed out.");
            Console.WriteLine($"Bus Inactive : {DateTime.Now}");
        }

        IBusActivityMonitor _activityMonitor;
        static IEnumerator<IRetryPolicy> RetryEnumerator => GetNextRetryPolicy().GetEnumerator();

        const int RetryPoliciesLength = 3;

        static IEnumerable<IRetryPolicy> GetNextRetryPolicy()
        {
            while (true)
            {
                foreach (var retryPolicy in new[] { Retry.None, Retry.Interval(3, TimeSpan.FromMilliseconds(50)), Retry.Immediate(3) })
                    yield return retryPolicy;
            }
        }

        protected override void ConnectObservers(IBus bus)
        {
            _activityMonitor = bus.CreateBusActivityMonitor(TimeSpan.FromMilliseconds(500));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            RetryEnumerator.MoveNext();
            var retryPolicy = RetryEnumerator.Current;
        #pragma warning disable 618
            configurator.UseRetry(r => r.SetRetryPolicy(x => retryPolicy));
        #pragma warning restore 618
            configurator.Consumer<TConsumer>(
                x =>
                {
                });
        }

        [OneTimeTearDown]
        public void BusActivityMonitor_SpecsTeardown()
        {
            RetryEnumerator?.Dispose();
        }

        async Task ActivityTask()
        {
            Console.WriteLine($"Activity Began : {DateTime.Now}");
            for (var i = 0; i < 10; i++)
            {
                var eventMessage = new PingMessage(Guid.NewGuid());
                await InputQueueSendEndpoint.Send(eventMessage).ConfigureAwait(false);
            }

            Console.WriteLine($"Activity Ended : {DateTime.Now}");
        }
    }


    public class SuccessConsumer :
        IConsumer<PingMessage>
    {
        public Task Consume(ConsumeContext<PingMessage> context)
        {
            return Task.CompletedTask;
        }
    }


    public class ThrowConsumer :
        IConsumer<PingMessage>
    {
        public Task Consume(ConsumeContext<PingMessage> context)
        {
            throw new ConsumerException("Consumer always throws!");
        }
    }


    public class RandomConsumer :
        IConsumer<PingMessage>
    {
        readonly ThreadSafeRandom _random = new ThreadSafeRandom();

        public Task Consume(ConsumeContext<PingMessage> context)
        {
            if (_random.NextBool())
                throw new ConsumerException("Consumer randomly throws!");
            return Task.CompletedTask;
        }
    }
}
