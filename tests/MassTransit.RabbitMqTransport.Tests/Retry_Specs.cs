namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_specifying_retry_limit :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            var pingId = NewId.NextGuid();
            _attempts[pingId] = 0;

            Task<ConsumeContext<Fault<PingMessage>>> handler =
                ConnectPublishHandler<Fault<PingMessage>>(context => context.Message.Message.CorrelationId == pingId);

            await Bus.Publish(new PingMessage(pingId));

            ConsumeContext<Fault<PingMessage>> handled = await handler;

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.GreaterThan(0));

            await InactivityTask.ContinueWith(task =>
            {
            });

            Assert.LessOrEqual(_attempts[pingId], _limit + 1);
        }

        readonly int _limit;

        readonly IDictionary<Guid, int> _attempts;

        public When_specifying_retry_limit()
        {
            TestInactivityTimeout = TimeSpan.FromSeconds(3);
            _limit = 2;
            _attempts = new Dictionary<Guid, int>();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            var sec2 = TimeSpan.FromSeconds(2);
            configurator.UseRetry(x => x.Exponential(_limit, sec2, sec2, sec2));

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
    public class When_specifying_redelivery_limit :
        RabbitMqTestFixture
    {
        [Category("Flaky")]
        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            var pingId = NewId.NextGuid();
            _attempts[pingId] = 0;

            Task<ConsumeContext<Fault<PingMessage>>> handler =
                ConnectPublishHandler<Fault<PingMessage>>(context => context.Message.Message.CorrelationId == pingId);

            await Bus.Publish(new PingMessage(pingId));

            ConsumeContext<Fault<PingMessage>> handled = await handler;

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.GreaterThan(0));

            await InactivityTask.ContinueWith(task =>
            {
            });

            Assert.LessOrEqual(_attempts[pingId], _limit + 1);
        }

        readonly int _limit;
        readonly IDictionary<Guid, int> _attempts;

        public When_specifying_redelivery_limit()
        {
            TestInactivityTimeout = TimeSpan.FromSeconds(3);
            _limit = 1;
            _attempts = new Dictionary<Guid, int>();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedExchangeMessageScheduler();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.UseScheduledRedelivery(x => x.Interval(_limit, TimeSpan.FromSeconds(1)));

            configurator.Consumer(() => new RetryLimitConsumer(_attempts));
        }
    }


    [TestFixture]
    public class When_specifying_redelivery_limit_with_message_ttl :
        RabbitMqTestFixture
    {
        [Category("Flaky")]
        [Test]
        public async Task Should_stop_after_limit_exceeded()
        {
            var pingId = NewId.NextGuid();
            _attempts[pingId] = 0;

            Task<ConsumeContext<Fault<PingMessage>>> handler =
                ConnectPublishHandler<Fault<PingMessage>>(context => context.Message.Message.CorrelationId == pingId);

            await Bus.Publish(new PingMessage(pingId), x => x.TimeToLive = TimeSpan.FromSeconds(2));

            ConsumeContext<Fault<PingMessage>> handled = await handler;

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.GreaterThan(0));

            await InactivityTask.ContinueWith(task =>
            {
            });

            Assert.LessOrEqual(_attempts[pingId], _limit + 1);
        }

        readonly int _limit;
        readonly IDictionary<Guid, int> _attempts;

        public When_specifying_redelivery_limit_with_message_ttl()
        {
            TestInactivityTimeout = TimeSpan.FromSeconds(3);
            _limit = 1;
            _attempts = new Dictionary<Guid, int>();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedExchangeMessageScheduler();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.UseScheduledRedelivery(x => x.Interval(_limit, TimeSpan.FromSeconds(1)));

            configurator.Consumer(() => new RetryLimitConsumer(_attempts));
        }
    }
}
