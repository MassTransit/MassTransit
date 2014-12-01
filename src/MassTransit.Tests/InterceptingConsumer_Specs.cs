// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Transactions;
    using BusConfigurators;
    using Magnum.Extensions;
    using MassTransit.Pipeline;
    using NUnit.Framework;
    using TextFixtures;


    [TestFixture]
    public class Intercepting_a_consumer_factory :
        LoopbackTestFixture
    {
        [Test]
        public void Should_properly_encapsulate_the_consumer_invocation()
        {
            LocalBus.Publish(new A());

            Assert.IsTrue(_myConsumer.Called.Task.Wait(8.Seconds()), "Consumer not called");
            Assert.IsTrue(_interceptor.First.Task.Wait(8.Seconds()), "First interceptor not called");
            Assert.IsTrue(_interceptor.Second.Task.Wait(8.Seconds()), "Second interceptor not called");
        }

        MyConsumer _myConsumer;
        Interceptor _interceptor;

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            _myConsumer = new MyConsumer();
            _interceptor = new Interceptor();

//            configurator.Subscribe(x =>
//            {
//                x.Consumer(() => _myConsumer)
//                    .Filter(_interceptor);
//            });
        }


        class Interceptor :
            IFilter<ConsumerConsumeContext<MyConsumer>>
        {
            public readonly TaskCompletionSource<bool> First = new TaskCompletionSource<bool>();
            public readonly TaskCompletionSource<bool> Second = new TaskCompletionSource<bool>();


            public async Task Send(ConsumerConsumeContext<MyConsumer> context, IPipe<ConsumerConsumeContext<MyConsumer>> next)
            {
                First.TrySetResult(true);
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    await next.Send(context);

                    scope.Complete();
                }
                Second.TrySetResult(true);
            }

            public bool Inspect(IPipeInspector inspector)
            {
                return inspector.Inspect(this);
            }
        }


        class MyConsumer :
            IConsumer<A>
        {
            public readonly TaskCompletionSource<A> Called = new TaskCompletionSource<A>();

            public async Task Consume(ConsumeContext<A> message)
            {
                Called.TrySetResult(message.Message);
            }
        }


        class A
        {
        }
    }
}