// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Configuring_a_filter_on_a_consumer_factory :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_invoke_the_filter()
        {
            await _requestClient.GetResponse<PongMessage>(new PingMessage());
        }

        IRequestClient<PingMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.Consumer<Consumer>(x =>
            {
                x.UseFilter(new UnitOfWorkFilter<Consumer>());
            });
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                var transaction = context.GetPayload<IUnitOfWork>();

                Console.WriteLine("Using transaction");

                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }


        class UnitOfWorkFilter<T> :
            IFilter<ConsumerConsumeContext<T>>
            where T : class
        {
            public async Task Send(ConsumerConsumeContext<T> context, IPipe<ConsumerConsumeContext<T>> next)
            {
                var unitOfWork = context.GetOrAddPayload<IUnitOfWork>(() => new UnitOfWork());

                try
                {
                    await next.Send(context).ConfigureAwait(false);

                    unitOfWork.Commit();
                }
                catch
                {
                    unitOfWork.Abandon();
                    throw;
                }
            }

            public void Probe(ProbeContext context)
            {
                context.CreateFilterScope("unitOfWork");
            }
        }


        class UnitOfWork :
            IUnitOfWork
        {
            readonly ITransaction _transaction;

            public UnitOfWork()
            {
                _transaction = new Transaction();
            }

            public ITransaction Transaction
            {
                get { return _transaction; }
            }

            public void Abandon()
            {
                Console.WriteLine("Abandoning Work");
            }

            public void Commit()
            {
                Console.WriteLine("Committing Work");
            }
        }


        interface IUnitOfWork
        {
            ITransaction Transaction { get; }

            void Abandon();
            void Commit();
        }


        interface ITransaction
        {
        }


        class Transaction :
            ITransaction
        {
        }
    }
}
