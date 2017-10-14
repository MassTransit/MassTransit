// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using GreenPipes;
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
                    using (TransactionScope scope = payload.CreateTransactionScope())
                    {
                        Console.WriteLine("ExecuteAsync: {0}", Thread.CurrentThread.ManagedThreadId);

                        Assert.IsNotNull(Transaction.Current);
                        Console.WriteLine("Isolation Level: {0}", Transaction.Current.IsolationLevel);
                    }
                }));
            });

            var context = new TestConsumeContext<PingMessage>(new PingMessage());

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<TransactionAbortedException>());

            //Console.WriteLine(exception.Message);
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

                    using (TransactionScope scope = payload.CreateTransactionScope())
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

            //Console.WriteLine(exception.Message);
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
                    using (TransactionScope scope = payload.CreateTransactionScope())
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

                    using (TransactionScope scope = payload.CreateTransactionScope())
                    {
                        Console.WriteLine("ExecuteAsync: {0}", Thread.CurrentThread.ManagedThreadId);

                        Assert.IsNotNull(Transaction.Current);
                        Console.WriteLine("Isolation Level: {0}", Transaction.Current.IsolationLevel);

                        Thread.Sleep(2000);

                        scope.Complete();
                    }

                    Console.WriteLine("Exited Scope");
                });
                x.UseExecute(payload => Console.WriteLine("After Transaction: {0}", Thread.CurrentThread.ManagedThreadId));
            });

            var context = new TestConsumeContext<PingMessage>(new PingMessage());

            Assert.That(async () => await pipe.Send(context), Throws.TypeOf<TransactionAbortedException>());

            //Console.WriteLine(exception.Message);
        }
    }


    [TestFixture]
    public class When_a_transaction_throws_an_exception_with_retry :
        InMemoryTestFixture
    {
        Task<ConsumeContext<Fault<Message>>> _faultReceived;

        [Test]
        public async Task Should_not_reuse_transaction_context_between_retries()
        {
            var fault = await _faultReceived;

            Assert.That(fault.Message.Exceptions.First().ExceptionType, Does.Contain(nameof(IntentionalTestException)));
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new Message {Id = NewId.NextGuid()});
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

                var isolationLevel = transactionContext.Transaction.IsolationLevel;

                using (var ts = new TransactionScope(transactionContext.Transaction))
                {
                    throw new IntentionalTestException("Then, you, shall, die!");
                }
            }
        }


        public class Message
        {
            public Guid Id { get; set; }
        }
    }
}