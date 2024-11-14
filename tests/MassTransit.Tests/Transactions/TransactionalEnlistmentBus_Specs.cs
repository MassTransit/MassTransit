namespace MassTransit.Tests.Transactions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using Internals;
    using MassTransit.Testing;
    using MassTransit.Transactions;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_transaction_scope_with_publish_and_complete
    {
        [Test]
        public async Task Should_support_the_test_harness()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTransactionalEnlistmentBus();

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(new PingMessage());

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);

                transaction.Complete();
            }

            Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }
    }


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

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032

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

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032

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

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032

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

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }
}
