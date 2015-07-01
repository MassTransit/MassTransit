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
namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Monitoring.Introspection;
    using NUnit.Framework;
    using TestFramework;


    public class Intercepting_a_consumer_factory :
        InMemoryTestFixture
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Await(() => InputQueueSendEndpoint.Send(new A()));
        }

        MyConsumer _myConsumer;
        TransactionFilter _transactionFilter;

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _myConsumer = new MyConsumer(GetTask<A>());
            _transactionFilter = new TransactionFilter(GetTask<bool>(), GetTask<bool>());

            configurator.Consumer(() => _myConsumer, x => x.UseFilter(_transactionFilter));
        }


        class TransactionFilter :
            IFilter<ConsumerConsumeContext<MyConsumer>>
        {
            public readonly TaskCompletionSource<bool> First;
            public readonly TaskCompletionSource<bool> Second;

            public TransactionFilter(TaskCompletionSource<bool> first, TaskCompletionSource<bool> second)
            {
                First = first;
                Second = second;
            }

            async Task IProbeSite.Probe(ProbeContext context)
            {
            }

            public async Task Send(ConsumerConsumeContext<MyConsumer> context, IPipe<ConsumerConsumeContext<MyConsumer>> next)
            {
                First.TrySetResult(true);

                await next.Send(context);

                Second.TrySetResult(true);
            }
        }


        class MyConsumer :
            IConsumer<A>
        {
            public readonly TaskCompletionSource<A> Called;

            public MyConsumer(TaskCompletionSource<A> called)
            {
                Called = called;
            }

            public async Task Consume(ConsumeContext<A> message)
            {
                Called.TrySetResult(message.Message);
            }
        }


        class A
        {
        }


        [Test]
        public async void Should_call_the_consumer_method()
        {
            await _myConsumer.Called.Task;
        }

        [Test]
        public async void Should_call_the_interceptor_first()
        {
            await _transactionFilter.First.Task;
        }

        [Test]
        public async void Should_call_the_interceptor_second()
        {
            await _transactionFilter.Second.Task;
        }
    }
}