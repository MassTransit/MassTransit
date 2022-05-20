namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    [Category("Flaky")]
    public class When_using_delayed_redelivery_at_the_bus_level :
        AmazonSqsTestFixture
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

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRedeliveryCount), Is.EqualTo(_limit));

            Assert.That(handled.Headers.Get<int>(MessageHeaders.FaultRetryCount), Is.EqualTo(1));

            await InactivityTask;

            Assert.LessOrEqual(_attempts[pingId], (_limit + 1) * 2);
        }

        readonly int _limit;
        readonly IDictionary<Guid, int> _attempts;

        public When_using_delayed_redelivery_at_the_bus_level()
        {
            _limit = 3;
            _attempts = new Dictionary<Guid, int>();
        }

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedRedelivery(x => x.Interval(_limit, TimeSpan.FromSeconds(1)));
            configurator.UseMessageRetry(x => x.Immediate(1));
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new RetryLimitConsumer(_attempts));
        }


        class RetryLimitConsumer :
            IConsumer<PingMessage>
        {
            readonly IDictionary<Guid, int> _attempts;

            public RetryLimitConsumer(IDictionary<Guid, int> attempts)
            {
                _attempts = attempts;
            }

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                _attempts[context.Message.CorrelationId]++;


                throw new IntentionalTestException();
            }
        }
    }
}
