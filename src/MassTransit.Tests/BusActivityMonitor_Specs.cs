namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Indicators;
    using TestFramework.Messages;
    using Util;


    
    [TestFixture(TypeArgs = new [] { typeof(SuccessConsumer) })]
    [TestFixture(TypeArgs = new [] { typeof(ThrowConsumer) })]
    [TestFixture(TypeArgs = new [] { typeof(RandomConsumer) })]
    public class BusActivityMonitor_Specs<TConsumer> :
        BusActivityMonitor_Specs
        where TConsumer : class, IConsumer<PingMessage>, new()
    {
        
        IBusActivityMonitor _activityMonitor;
        IEnumerator<IRetryPolicy> _retryEnumerator; 

        readonly static IRetryPolicy[] retryPolicies = {
            Retry.None,
            Retry.Interval(3, TimeSpan.FromMilliseconds(50)),
            Retry.Immediate(3)
        };

        const int RetryPoliciesLength = 3;

        IEnumerable<IRetryPolicy> GetNextRetryPolicy()
        {
            while (true)
            {
                foreach (var retryPolicy in retryPolicies)
                    yield return retryPolicy;
            }
        }
        
        protected override void ConnectObservers(IBus bus)
        {
            _activityMonitor = bus.CreateBusActivityMonitor(TimeSpan.FromMilliseconds(500));
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _retryEnumerator.MoveNext();
            var retryPolicy = _retryEnumerator.Current;
            configurator.UseRetry(retryPolicy);
            configurator.Consumer<TConsumer>(
                x => { });
            
        }

        [TestFixtureSetUp]
        public void BusActivityMonitor_SpecsSetup()
        {
            _retryEnumerator = GetNextRetryPolicy().GetEnumerator();
        }

        [TestFixtureTearDown]
        public void BusActivityMonitor_SpecsTeardown()
        {
            _retryEnumerator?.Dispose();
        }

        [Test, Repeat(RetryPoliciesLength)]
        public async Task Should_detect_inactivity()
        {
#pragma warning disable 4014
            ActivityTask();
#pragma warning restore 4014

            bool timeout = await _activityMonitor.AwaitBusInactivity(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            Assert.IsTrue(timeout, "Activity monitor timed out.");
            Console.WriteLine($"Bus Inactive : {DateTime.Now}");
        }

        async Task ActivityTask()
        {
            Console.WriteLine($"Activity Began : {DateTime.Now}");
            for (int i = 0; i < 10; i++)
            {
                PingMessage eventMessage = new PingMessage(Guid.NewGuid());
                await InputQueueSendEndpoint.Send(eventMessage).ConfigureAwait(false);
            }
            Console.WriteLine($"Activity Ended : {DateTime.Now}");
        }



    }


    public class BusActivityMonitor_Specs :
        InMemoryTestFixture
    {
        public BusActivityMonitor_Specs() :
            base(true)
        { }

        
    }

    public class SuccessConsumer :
    IConsumer<PingMessage>
    {
        public Task Consume(ConsumeContext<PingMessage> context)
        {
            return TaskUtil.Completed;
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
            return TaskUtil.Completed;
        }
    }

}