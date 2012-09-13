// Copyright 2007-2010 The Apache Software Foundation.
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
    using System.Diagnostics;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Tests;
    using MassTransit.Tests.Messages;
    using NUnit.Framework;
    using TestFixtures;
    using TestFramework;

    [TestFixture, Integration]
    public class Given_a_message_is_received_from_a_nontransactional_queue :
        MsmqEndpointTestFixture
    {
    }

    [TestFixture, Integration]
    public class When_a_consumer_throws_an_exception :
        Given_a_message_is_received_from_a_nontransactional_queue
    {
        private PingMessage _ping;
        private FutureMessage<Fault<PingMessage, Guid>> _faultFuture;
        private FutureMessage<PingMessage, Guid> _future;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _ping = new PingMessage();

            _faultFuture = new FutureMessage<Fault<PingMessage, Guid>>();
            _future = new FutureMessage<PingMessage, Guid>(_ping.CorrelationId);

            LocalBus.SubscribeHandler<PingMessage>(message =>
                {
                    _future.Set(message);

                    throw new NotSupportedException("I am a naughty consumer! I go boom!");
                });

            LocalBus.SubscribeHandler<Fault<PingMessage, Guid>>(message => _faultFuture.Set(message));

            LocalBus.Publish(_ping);

            _future.WaitUntilAvailable(1.Seconds());
        }

        [Test]
        public void The_message_should_exist_in_the_error_queue()
        {
            LocalErrorEndpoint.ShouldContain(_ping, Debugger.IsAttached ? 300.Seconds() : 5.Seconds());
        }

        [Test]
        public void The_message_should_not_exist_in_the_input_queue()
        {
            LocalEndpoint.ShouldNotContain(_ping);
        }

        [Test]
        public void A_fault_should_be_published()
        {
            _faultFuture.IsAvailable(3.Seconds())
                .ShouldBeTrue();
        }
    }
    
    [TestFixture, Integration]
    public class When_a_consumer_receives_the_message :
        Given_a_message_is_received_from_a_nontransactional_queue
    {
        private PingMessage _ping;
        private FutureMessage<PingMessage> _future;

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
            _future.IsAvailable(3.Seconds())
                .ShouldBeTrue();

            LocalErrorEndpoint.ShouldNotContain(_ping);
        }

        [Test]
        public void The_message_should_not_exist_in_the_input_queue()
        {
            _future.IsAvailable(3.Seconds())
                .ShouldBeTrue();

            LocalEndpoint.ShouldNotContain(_ping);
        }
    }
}