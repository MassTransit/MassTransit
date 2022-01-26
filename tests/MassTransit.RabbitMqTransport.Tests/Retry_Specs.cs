namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    [Category("Flaky")]
    public class When_specifying_retry_limit :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            var pingId = NewId.NextGuid();
            _attempts[pingId] = 0;

            Task<ConsumeContext<Fault<PingMessage>>> handler =
                await ConnectPublishHandler<Fault<PingMessage>>(context => context.Message.Message.CorrelationId == pingId);

            await Bus.Publish(new PingMessage(pingId));

            ConsumeContext<Fault<PingMessage>> handled = await handler;

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.GreaterThan(0));

            await InactivityTask;

            Assert.LessOrEqual(_attempts[pingId], _limit + 1);
        }

        readonly int _limit;

        readonly IDictionary<Guid, int> _attempts;

        public When_specifying_retry_limit()
        {
            _limit = 2;
            _attempts = new Dictionary<Guid, int>();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(x => x.Interval(_limit, 200));

            configurator.Consumer(() => new RetryLimitConsumer(_attempts));
        }
    }


    class RetryLimitConsumer :
        IConsumer<PingMessage>
    {
        readonly IDictionary<Guid, int> _attempts;

        public RetryLimitConsumer(IDictionary<Guid, int> attempts)
        {
            _attempts = attempts;
        }

        public Task Consume(ConsumeContext<PingMessage> context)
        {
            _attempts[context.Message.CorrelationId]++;
            throw new IntentionalTestException();
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class When_specifying_redelivery_limit :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            var pingId = NewId.NextGuid();
            _attempts[pingId] = 0;

            Task<ConsumeContext<Fault<PingMessage>>> handler =
                await ConnectPublishHandler<Fault<PingMessage>>(context => context.Message.Message.CorrelationId == pingId);

            await Bus.Publish(new PingMessage(pingId));

            ConsumeContext<Fault<PingMessage>> handled = await handler;

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.GreaterThan(0));

            await InactivityTask;

            Assert.LessOrEqual(_attempts[pingId], _limit + 1);
        }

        readonly int _limit;
        readonly IDictionary<Guid, int> _attempts;

        public When_specifying_redelivery_limit()
        {
            _limit = 1;
            _attempts = new Dictionary<Guid, int>();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.UseScheduledRedelivery(x => x.Interval(_limit, TimeSpan.FromSeconds(1)));

            configurator.Consumer(() => new RetryLimitConsumer(_attempts));
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class When_specifying_redelivery_at_the_bus_level :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            var pingId = NewId.NextGuid();
            _attempts[pingId] = 0;

            Task<ConsumeContext<Fault<PingMessage>>> handler =
                await ConnectPublishHandler<Fault<PingMessage>>(context => context.Message.Message.CorrelationId == pingId);

            await Bus.Publish(new PingMessage(pingId));

            ConsumeContext<Fault<PingMessage>> handled = await handler;

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.GreaterThan(0));

            await InactivityTask;

            Assert.LessOrEqual(_attempts[pingId], _limit + 1);
        }

        readonly int _limit;
        readonly IDictionary<Guid, int> _attempts;

        public When_specifying_redelivery_at_the_bus_level()
        {
            _limit = 1;
            _attempts = new Dictionary<Guid, int>();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();

            configurator.UseScheduledRedelivery(x => x.Interval(_limit, TimeSpan.FromSeconds(1)));
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new RetryLimitConsumer(_attempts));
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class When_using_delayed_redelivery_at_the_bus_level :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            var pingId = NewId.NextGuid();
            _attempts[pingId] = 0;

            Task<ConsumeContext<Fault<PingMessage>>> handler =
                await ConnectPublishHandler<Fault<PingMessage>>(context => context.Message.Message.CorrelationId == pingId);

            await Bus.Publish(new PingMessage(pingId));

            ConsumeContext<Fault<PingMessage>> handled = await handler;

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.GreaterThan(0));

            await InactivityTask;

            Assert.LessOrEqual(_attempts[pingId], _limit + 1);
        }

        readonly int _limit;
        readonly IDictionary<Guid, int> _attempts;

        public When_using_delayed_redelivery_at_the_bus_level()
        {
            _limit = 1;
            _attempts = new Dictionary<Guid, int>();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedRedelivery(x => x.Interval(_limit, TimeSpan.FromSeconds(1)));
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new RetryLimitConsumer(_attempts));
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class When_specifying_redelivery_limit_with_message_ttl :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            var pingId = NewId.NextGuid();
            _attempts[pingId] = 0;

            Task<ConsumeContext<Fault<PingMessage>>> handler =
                await ConnectPublishHandler<Fault<PingMessage>>(context => context.Message.Message.CorrelationId == pingId);

            await Bus.Publish(new PingMessage(pingId), x => x.TimeToLive = TimeSpan.FromSeconds(2));

            ConsumeContext<Fault<PingMessage>> handled = await handler;

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.GreaterThan(0));

            await InactivityTask;

            Assert.LessOrEqual(_attempts[pingId], _limit + 1);
        }

        readonly int _limit;
        readonly IDictionary<Guid, int> _attempts;

        public When_specifying_redelivery_limit_with_message_ttl()
        {
            _limit = 1;
            _attempts = new Dictionary<Guid, int>();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.UseScheduledRedelivery(x => x.Interval(_limit, TimeSpan.FromSeconds(1)));

            configurator.Consumer(() => new RetryLimitConsumer(_attempts));
        }
    }
}
