namespace MassTransit.Tests.Transactions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using MassTransit.Transactions;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_using_transactional_bus_Sendendpoint
    {
        [Test]
        public async Task Should_throw_exception_immediately_outside_transaction()
        {
            var message = new PingMessage();

            var sendEndpoint = new TransactionalBusSendEndpoint(new TransactionalEnlistmentBus(null), new AlwaysThrowSendEndpointMock());
            Assert.That(async () => await sendEndpoint.Send(message), Throws.TypeOf<ExceptionToBeCaught>());
        }

        [Test]
        public async Task Should_throw_exception_when_transaction_commited_and_disposed()
        {
            var message = new PingMessage();

            var sendEndpoint = new TransactionalBusSendEndpoint(new TransactionalEnlistmentBus(null), new AlwaysThrowSendEndpointMock());
            Assert.That(async () =>
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await sendEndpoint.Send(message);

                    transaction.Complete();
                }
            }, Throws.TypeOf<TransactionAbortedException>().And.InnerException.TypeOf<ExceptionToBeCaught>());
        }


        class ExceptionToBeCaught : Exception
        {
        }


        class AlwaysThrowSendEndpointMock : ISendEndpoint
        {
            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                throw new NotImplementedException();
            }

            public async Task Send<T>(T message, CancellationToken cancellationToken = default)
                where T : class
            {
                await Task.Delay(100); //fake doing some work before throwing exception
                throw new ExceptionToBeCaught();
            }

            public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                throw new NotImplementedException();
            }

            public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                throw new NotImplementedException();
            }

            public Task Send(object message, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task Send(object message, Type messageType, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task Send<T>(object values, CancellationToken cancellationToken = default)
                where T : class
            {
                throw new NotImplementedException();
            }

            public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                throw new NotImplementedException();
            }

            public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
                where T : class
            {
                throw new NotImplementedException();
            }
        }
    }
}
