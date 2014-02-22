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
namespace MassTransit.Tests.Configuration
{
    using System.Linq;
    using Magnum.TestFramework;
    using MassTransit.Configuration;
    using SubscriptionConnectors;


    [Scenario]
    public class When_a_consumer_with_consumes_all_interfaces_is_inspected
    {
        DelegateConsumerFactory<Consumer> _consumerFactory;
        ConsumerConnector<Consumer> _factory;

        [When]
        public void A_consumer_with_consumes_all_interfaces_is_inspected()
        {
            _consumerFactory = new DelegateConsumerFactory<Consumer>(() => new Consumer());

            _factory = new ConsumerConnector<Consumer>();
        }

        [Then]
        public void Should_create_the_builder()
        {
            _factory.ShouldNotBeNull();
        }

        [Then]
        public void Should_have_four_subscription_types()
        {
            _factory.Connectors.Count().ShouldEqual(4);
        }

        [Then]
        public void Should_have_an_a()
        {
            _factory.Connectors.First().MessageType.ShouldEqual(typeof(A));
        }

        [Then]
        public void Should_have_a_b()
        {
            _factory.Connectors.Skip(1).First().MessageType.ShouldEqual(typeof(B));
        }

        [Then]
        public void Should_have_a_c()
        {
            _factory.Connectors.Skip(2).First().MessageType.ShouldEqual(typeof(IC));
        }

        [Then]
        public void Should_have_a_d()
        {
            _factory.Connectors.Skip(3).First().MessageType.ShouldEqual(typeof(D<A>));
        }


        class A
        {
        }


        class B
        {
        }


        class Consumer :
            Consumes<A>.All,
            Consumes<B>.All,
            Consumes<IC>.All,
            Consumes<D<A>>.All
        {
            public void Consume(A message)
            {
            }

            public void Consume(B message)
            {
            }

            public void Consume(D<A> message)
            {
            }

            public void Consume(IC message)
            {
            }
        }


        class D<T>
        {
        }


        interface IC
        {
        }
    }
}