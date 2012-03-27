// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Subscriptions
{
    using System;
    using BusConfigurators;
    using Magnum.Extensions;
    using NUnit.Framework;
    using TestConsumers;
    using TestFramework;
    using TextFixtures;

    [TestFixture]
    public class A_consumer_with_two_message_contracts :
        LoopbackLocalAndRemoteTestFixture
    {
        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Subscribe(x => { x.Consumer<ConsumerOfAAndB>(); });
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();

            LocalBus.ShouldHaveSubscriptionFor<A>();
            LocalBus.ShouldHaveSubscriptionFor<B>();
        }

        class ConsumerOfAAndB :
            Consumes<A>.All,
            Consumes<B>.All
        {
            readonly TestConsumerBase<A> _consumerA;
            readonly TestConsumerBase<B> _consumerB;

            public ConsumerOfAAndB()
            {
                _consumerA = new TestConsumerBase<A>();
                _consumerB = new TestConsumerBase<B>();
            }

            public TestConsumerBase<A> ConsumerA
            {
                get { return _consumerA; }
            }

            public TestConsumerBase<B> ConsumerB
            {
                get { return _consumerB; }
            }

            public void Consume(A message)
            {
                _consumerA.Consume(message);
            }

            public void Consume(B message)
            {
                _consumerB.Consume(message);
            }
        }

        class A
        {
            public A()
            {
                Value = Guid.NewGuid();
            }

            public Guid Value { get; private set; }

            public bool Equals(A other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return other.Value.Equals(Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof(A))
                    return false;
                return Equals((A)obj);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }

        class B
        {
            public B()
            {
                Value = Guid.NewGuid();
            }

            public Guid Value { get; private set; }

            public bool Equals(B other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return other.Value.Equals(Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof(B))
                    return false;
                return Equals((B)obj);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }

        [Test]
        public void Should_have_received_a()
        {
            var message = new A();
            LocalBus.Publish(message);

            TestConsumerBase<A>.OnlyOneShouldHaveReceivedMessage(message, 8.Seconds());
        }

        [Test]
        public void Should_have_received_b()
        {
            var message = new B();
            RemoteBus.Publish(message);

            TestConsumerBase<B>.OnlyOneShouldHaveReceivedMessage(message, 8.Seconds());
        }
    }
}