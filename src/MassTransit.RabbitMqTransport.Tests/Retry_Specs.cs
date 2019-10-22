namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using TestFramework.Messages;
    using NUnit.Framework;
    using GreenPipes;
    using MassTransit.Testing;
    using MassTransit.Testing.Indicators;
    using TestFramework;


    [TestFixture]
    public class When_specifying_retry_limit :
        RabbitMqTestFixture
    {
        readonly int _limit;
        int _attempts;
        IBusActivityMonitor _activityMonitor;
        Task<ConsumeContext<Fault<PingMessage>>> _handled;

        public When_specifying_retry_limit()
        {
            _limit = 2;
            _attempts = 0;
        }

        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            await Bus.Publish(new PingMessage());

            var handled = await _handled;

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.GreaterThan(0));

            await _activityMonitor.AwaitBusInactivity(TestCancellationToken);

            Assert.LessOrEqual(_attempts, _limit + 1);
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(e =>
            {
                _handled = Handled<Fault<PingMessage>>(e);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            var sec2 = TimeSpan.FromSeconds(2);
            configurator.UseRetry(x => x.Exponential(_limit, sec2, sec2, sec2));

            configurator.Consumer(() => new Consumer(ref _attempts));
        }

        protected override void ConnectObservers(IBus bus)
        {
            base.ConnectObservers(bus);

            _activityMonitor = bus.CreateBusActivityMonitor(TimeSpan.FromMilliseconds(500));
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public Consumer(ref int attempts)
            {
                ++attempts;
            }

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                throw new IntentionalTestException();
            }
        }
    }
}
