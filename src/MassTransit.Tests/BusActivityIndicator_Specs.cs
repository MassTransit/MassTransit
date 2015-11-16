namespace MassTransit.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Indicators;
    using TestFramework.Messages;
    using Util;
    

    [TestFixture(TypeArgs = new []{ typeof(BusActivityIndicator_Specs<>.SuccessConsumer) })]
    public class BusActivityIndicator_Specs<TConsumer> :
        InMemoryTestFixture
        where TConsumer : class, IConsumer<PingMessage>, new()
    {
        public static readonly BusActivityTestConfiguration<TConsumer> NoRetry = new BusActivityTestConfiguration<TConsumer>(Retry.None);
        public static readonly BusActivityTestConfiguration<TConsumer> IntervalRetry = new BusActivityTestConfiguration<TConsumer>(Retry.Interval(3, TimeSpan.FromMilliseconds(50)));
        public static readonly BusActivityTestConfiguration<TConsumer> ImmediateRetry = new BusActivityTestConfiguration<TConsumer>(Retry.Immediate(5));

        readonly BusActivityTestConfiguration<TConsumer> _configuration;
        IBusActivityMonitor _activityMonitor;

        public BusActivityIndicator_Specs(TestScenario scenario)
        {
            switch (scenario)
            {
                case TestScenario.NoRetry:
                    _configuration = NoRetry;
                    break;
                case TestScenario.IntervalRetry:
                    _configuration = IntervalRetry;
                    break;
                case TestScenario.ImmediateRetry:
                    _configuration = ImmediateRetry;
                    break;
                default:
                    throw new ArgumentException("Invalid Test Scenario.");
            }
        }

        protected override void ConnectObservers(IBus bus)
        {
            _activityMonitor = bus.CreateBusActivityMonitor();
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(_configuration.RetryPolicy);
            configurator.Consumer<TConsumer>(
                x =>
                {
                    x.UseConsoleLog(async (context, logContext) =>
                        string.Format("{1:u} {2:F0} Consumer: {0}", TypeMetadataCache<SuccessConsumer>.ShortName,
                            logContext.StartTime, logContext.Duration.TotalMilliseconds));
                });
        }

        [Test]
        public async Task Test_success()
        {
#pragma warning disable 4014
            ActivityTask();
#pragma warning restore 4014

            bool timeout = await _activityMonitor.AwaitBusInactivity(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            Assert.IsFalse(timeout, "Activity monitor timed out.");
            Console.WriteLine($"Bus Inactive : {DateTime.Now}");
        }

        async Task ActivityTask()
        {
            Console.WriteLine($"Activity Began : {DateTime.Now}");
            for (int i = 0; i < 100; i++)
            {
                PingMessage eventMessage = new PingMessage(Guid.NewGuid());
                await InputQueueSendEndpoint.Send(eventMessage).ConfigureAwait(false);
            }
            Console.WriteLine($"Activity Ended : {DateTime.Now}");
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


        public class BusActivityTestConfiguration<TConsumer1>
            where TConsumer1 : class, IConsumer<PingMessage>
        {
            public BusActivityTestConfiguration(IRetryPolicy retryPolicy)
            {
                RetryPolicy = retryPolicy;
            }

            public IRetryPolicy RetryPolicy { get; }
        }


        public enum TestScenario
        {
            NoRetry,
            IntervalRetry,
            ImmediateRetry
        }
    }
}