namespace MassTransit.Tests.Transactions
{
    using System;
    using System.Threading.Tasks;
    using System.Transactions;
    using Internals;
    using MassTransit.Transactions;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_using_transaction_scope_with_publish_and_complete :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_publish_properly()
        {
            var message = new PingMessage();
            var transactionOutbox = new TransactionalEnlistmentBus(Bus);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await transactionOutbox.Publish(message);

                // Hasn't published yet
                Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());

                transaction.Complete();
            }

            // Now has published
            await _received;
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class When_using_transaction_scope_with_send_and_complete :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_send_properly()
        {
            var message = new PingMessage();
            var transactionOutbox = new TransactionalEnlistmentBus(Bus);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var sendEndpoint = await transactionOutbox.GetSendEndpoint(InputQueueAddress);
                await sendEndpoint.Send(message);

                Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());

                transaction.Complete();
            }

            await _received;
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class When_using_transaction_scope_with_publish_and_no_complete :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_publish_properly()
        {
            var message = new PingMessage();
            var transactionOutbox = new TransactionalEnlistmentBus(Bus);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await transactionOutbox.Publish(message);
            }

            Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class When_using_transaction_scope_with_send_and_no_complete :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_send_properly()
        {
            var message = new PingMessage();
            var transactionOutbox = new TransactionalEnlistmentBus(Bus);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var sendEndpoint = await transactionOutbox.GetSendEndpoint(InputQueueAddress);
                await sendEndpoint.Send(message);
            }

            Assert.That(async () => await _received.OrTimeout(s: 3), Throws.TypeOf<TimeoutException>());
        }

        Task<ConsumeContext<PingMessage>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }
}
