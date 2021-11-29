namespace MassTransit.Tests.Transactions
{
    using System;
    using System.Threading.Tasks;
    using Internals;
    using MassTransit.Transactions;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_using_bus_outbox_publish_when_released :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_properly()
        {
            var message = new PingMessage();
            var bus = new TransactionalBus(Bus);

            await bus.Publish(message);

            // Hasn't sent yet
            Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());

            await bus.Release();

            await _received;
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class When_using_bus_outbox_with_send_and_complete :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_send_properly()
        {
            var message = new PingMessage();
            var bus = new TransactionalBus(Bus);

            var sendEndpoint = await bus.GetSendEndpoint(InputQueueAddress);
            await sendEndpoint.Send(message);

            // Hasn't sent yet
            Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());

            await bus.Release();

            await _received;
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class When_using_bus_outbox_with_publish_and_no_complete :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_publish_properly()
        {
            var message = new PingMessage();
            var bus = new TransactionalBus(Bus);

            await bus.Publish(message);

            Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class When_using_bus_outbox_with_send_and_no_complete :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_send_properly()
        {
            var message = new PingMessage();
            var bus = new TransactionalBus(Bus);

            var sendEndpoint = await bus.GetSendEndpoint(InputQueueAddress);
            await sendEndpoint.Send(message);

            Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }
}
