namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_a_transaction_spans_threads
    {
        [Test]
        public void Should_properly_fail()
        {
            IPipe<ConsumeContext> pipe = Pipe.New<ConsumeContext>(x =>
            {
                x.UseTransaction();
                x.UseExecute(payload => Console.WriteLine("Execute: {0}", Thread.CurrentThread.ManagedThreadId));
                x.UseExecuteAsync(payload => Task.Run(() =>
                {
                    using (var scope = payload.CreateTransactionScope())
                    {
                        Console.WriteLine("ExecuteAsync: {0}", Thread.CurrentThread.ManagedThreadId);

                        Assert.IsNotNull(Transaction.Current);
                        Console.WriteLine("Isolation Level: {0}", Transaction.Current.IsolationLevel);
                    }
                }));
            });

            var context = new TestConsumeContext<PingMessage>(new PingMessage());

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<TransactionAbortedException>());
        }

        [Test]
        public void Should_properly_handle_exception()
        {
            IPipe<ConsumeContext> pipe = Pipe.New<ConsumeContext>(x =>
            {
                x.UseTransaction();
                x.UseExecute(payload => Console.WriteLine("Execute: {0}", Thread.CurrentThread.ManagedThreadId));
                x.UseExecuteAsync(async payload =>
                {
                    await Task.Yield();

                    using (var scope = payload.CreateTransactionScope())
                    {
                        Console.WriteLine("ExecuteAsync: {0}", Thread.CurrentThread.ManagedThreadId);

                        Assert.IsNotNull(Transaction.Current);
                        Console.WriteLine("Isolation Level: {0}", Transaction.Current.IsolationLevel);

                        scope.Complete();
                    }

                    throw new InvalidOperationException("This is a friendly boom");
                });
                x.UseExecute(payload => Console.WriteLine("After Transaction: {0}", Thread.CurrentThread.ManagedThreadId));
            });

            var context = new TestConsumeContext<PingMessage>(new PingMessage());

            Assert.That(async () => await pipe.Send(context), Throws.InvalidOperationException);
        }

        [Test]
        public async Task Should_properly_succeed()
        {
            IPipe<ConsumeContext> pipe = Pipe.New<ConsumeContext>(x =>
            {
                x.UseTransaction();
                x.UseExecute(payload => Console.WriteLine("Execute: {0}", Thread.CurrentThread.ManagedThreadId));
                x.UseExecuteAsync(payload => Task.Run(() =>
                {
                    using (var scope = payload.CreateTransactionScope())
                    {
                        Console.WriteLine("ExecuteAsync: {0}", Thread.CurrentThread.ManagedThreadId);

                        Assert.IsNotNull(Transaction.Current);
                        Console.WriteLine("Isolation Level: {0}", Transaction.Current.IsolationLevel);
                        scope.Complete();
                    }
                }));
            });

            var context = new TestConsumeContext<PingMessage>(new PingMessage());

            await pipe.Send(context);
        }

        [Test]
        public void Should_timeout()
        {
            IPipe<ConsumeContext> pipe = Pipe.New<ConsumeContext>(x =>
            {
                x.UseTransaction(t => t.Timeout = TimeSpan.FromSeconds(1));
                x.UseExecute(payload => Console.WriteLine("Execute: {0}", Thread.CurrentThread.ManagedThreadId));
                x.UseExecuteAsync(async payload =>
                {
                    await Task.Yield();

                    using (var scope = payload.CreateTransactionScope())
                    {
                        Console.WriteLine("ExecuteAsync: {0}", Thread.CurrentThread.ManagedThreadId);

                        Assert.IsNotNull(Transaction.Current);
                        Console.WriteLine("Isolation Level: {0}", Transaction.Current.IsolationLevel);

                        Thread.Sleep(5000);

                        scope.Complete();
                    }

                    Console.WriteLine("Exited Scope");
                });
                x.UseExecute(payload => Console.WriteLine("After Transaction: {0}", Thread.CurrentThread.ManagedThreadId));
            });

            var context = new TestConsumeContext<PingMessage>(new PingMessage());

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<TransactionAbortedException>());
        }
    }


    [TestFixture]
    public class When_a_transaction_throws_an_exception_with_retry :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_reuse_transaction_context_between_retries()
        {
            ConsumeContext<Fault<Message>> fault = await _faultReceived;

            Assert.That(fault.Message.Exceptions.First().ExceptionType, Does.Contain(nameof(IntentionalTestException)));
        }

        Task<ConsumeContext<Fault<Message>>> _faultReceived;

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new Message { Id = NewId.NextGuid() });
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("errors", x =>
            {
                _faultReceived = Handled<Fault<Message>>(x);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseRetry(x => x.Immediate(1));
            configurator.UseTransaction();

            configurator.Consumer<Consumer>();
        }


        class Consumer : IConsumer<Message>
        {
            public Task Consume(ConsumeContext<Message> context)
            {
                var transactionContext = context.GetPayload<TransactionContext>();

                using var ts = new TransactionScope(transactionContext.Transaction);

                throw new IntentionalTestException("Then, you, shall, die!");
            }
        }


        public class Message
        {
            public Guid Id { get; set; }
        }
    }
}
