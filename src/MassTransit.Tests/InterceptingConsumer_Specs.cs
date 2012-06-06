// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Transactions;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
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

            MyConsumer.Called.IsAvailable(8.Seconds()).ShouldBeTrue();

            _first.IsAvailable(8.Seconds()).ShouldBeTrue("Interceptor not called first");
            _second.IsAvailable(8.Seconds()).ShouldBeTrue("Interceptor not called second");
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Subscribe(x =>
                {
                    x.InterceptingConsumer(() => new MyConsumer(), consumer =>
                        {
                            _first.Set(1);
                            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                consumer();

                                scope.Complete();
                            }
                            _second.Set(2);
                        });
                });
        }

        FutureMessage<int> _first = new FutureMessage<int>();
        FutureMessage<int> _second = new FutureMessage<int>();

        class MyConsumer :
            Consumes<A>.Context
        {
            public static FutureMessage<A> Called = new FutureMessage<A>();

            public void Consume(IConsumeContext<A> message)
            {
                Called.Set(message.Message);
            }
        }

        class A
        {
        }
    }
}