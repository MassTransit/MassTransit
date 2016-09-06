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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using GreenPipes;
    using MassTransit.Pipeline;
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
}