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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Transactions;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Tests;
    using MassTransit.Tests.Messages;
    using NUnit.Framework;
    using TestFixtures;
    using TestFramework;

    [TestFixture, Integration]
    public class Given_a_message_is_received_from_a_transactional_queue :
        MsmqTransactionalEndpointTestFixture
    {
    }

    [TestFixture, Integration]
    public class When_a_consumer_throws_an_exception_in_the_transaction :
        Given_a_message_is_received_from_a_transactional_queue
    {
        PingMessage _ping;
        FutureMessage<Fault<PingMessage, Guid>> _faultFuture;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _faultFuture = new FutureMessage<Fault<PingMessage, Guid>>();

            LocalBus.SubscribeHandler<PingMessage>(
                message => { throw new NotSupportedException("I am a naughty consumer! I go boom!"); });
            LocalBus.SubscribeHandler<Fault<PingMessage, Guid>>(message =>
                {
                    if (_faultFuture.IsAvailable(TimeSpan.Zero))
                        return;

                    _faultFuture.Set(message);
                });

            LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
            LocalBus.ShouldHaveRemoteSubscriptionFor<Fault<PingMessage, Guid>>();

            _ping = new PingMessage();

            LocalBus.Publish(_ping);
        }

        [Test]
        public void A_fault_should_be_published()
        {
            _faultFuture.IsAvailable(3.Seconds()).ShouldBeTrue();
        }

        [Test]
        public void The_message_should_exist_in_the_error_queue()
        {
            LocalErrorEndpoint.ShouldContain(_ping, 5.Seconds());
        }

        [Test]
        public void The_message_should_not_exist_in_the_input_queue()
        {
            LocalEndpoint.ShouldNotContain(_ping);
        }
    }

    [TestFixture, Integration]
    public class When_a_consumer_receives_the_message_in_the_transaction :
        Given_a_message_is_received_from_a_transactional_queue
    {
        PingMessage _ping;
        FutureMessage<PingMessage> _future;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _future = new FutureMessage<PingMessage>();

            LocalBus.SubscribeHandler<PingMessage>(message => _future.Set(message));

            _ping = new PingMessage();

            LocalBus.Publish(_ping);
        }

        [Test]
        public void The_message_should_not_exist_in_the_error_queue()
        {
            _future.IsAvailable(3.Seconds()).ShouldBeTrue();

            LocalErrorEndpoint.ShouldNotContain(_ping);
        }

        [Test]
        public void The_message_should_not_exist_in_the_input_queue()
        {
            _future.IsAvailable(3.Seconds()).ShouldBeTrue();

            LocalEndpoint.ShouldNotContain(_ping);
        }
    }

    [TestFixture, Integration]
    public class When_a_consumer_does_not_complete_nested_transaction_in_the_root_transaction :
        Given_a_message_is_received_from_a_transactional_queue
    {
        PingMessage _ping;
        FutureMessage<Fault<PingMessage, Guid>> _faultFuture;


        public When_a_consumer_does_not_complete_nested_transaction_in_the_root_transaction()
        {
            ConfigureEndpointFactory(x => x.SetPurgeOnStartup(true)); // to get test worked after failing
        }

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _faultFuture = new FutureMessage<Fault<PingMessage, Guid>>();

            LocalBus.SubscribeHandler<PingMessage>(message =>
                {
                    using (new TransactionScope())
                    {
                    }
                });
            LocalBus.SubscribeHandler<Fault<PingMessage, Guid>>(message =>
                {
                    if (_faultFuture.IsAvailable(TimeSpan.Zero))
                        return;

                    _faultFuture.Set(message);
                });

            LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();
            LocalBus.ShouldHaveRemoteSubscriptionFor<Fault<PingMessage, Guid>>();

            _ping = new PingMessage();

            LocalBus.Publish(_ping);
        }

        /// <summary>
        /// This method is no longer a test, because we can't know the type of the message handler that
        /// crashed the transaction scope. So no fault is produced since it's not a consumer fault.
        /// Basically, we fixed the glitch with transaction scope but it's not reported as a Fault.
        /// </summary>
        public void A_fault_should_be_published()
        {
            _faultFuture.IsAvailable(8.Seconds()).ShouldBeTrue();
        }

        [Test]
        public void The_message_should_exist_in_the_error_queue()
        {
            LocalErrorEndpoint.ShouldContain(_ping, 8.Seconds());
        }

        [Test]
        public void The_message_should_not_exist_in_the_input_queue()
        {
            LocalEndpoint.ShouldNotContain(_ping);
            // it can pass because of infinite processing loop, however endpoint contains ping message
        }
    }
}