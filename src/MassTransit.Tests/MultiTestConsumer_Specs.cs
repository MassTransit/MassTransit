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
    using System;
    using System.Linq;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_the_multi_test_consumer :
        InMemoryTestFixture
    {
        [Test]
        public void Should_distinguish_multiple_events()
        {
            var consumer = new PingPongConsumer(TestTimeout);

            using (ConnectHandle handle = consumer.Connect(Bus))
            {
                var pingMessage = new PingMessage();
                var pingMessage2 = new PingMessage();
                Bus.Publish(pingMessage);
                Bus.Publish(pingMessage2);

                consumer.Received.Select<PingMessage>(received => received.Context.Message.CorrelationId == pingMessage.CorrelationId).Any().ShouldBe(true);
                consumer.Received.Select<PingMessage>(received => received.Context.Message.CorrelationId == pingMessage2.CorrelationId).Any().ShouldBe(true);
            }
        }

        [Test]
        public void Should_show_that_the_message_was_received_by_the_consumer()
        {
            var multiConsumer = new MultiTestConsumer(TestTimeout);
            ReceivedMessageList<PingMessage> received = multiConsumer.Consume<PingMessage>();

            using (ConnectHandle handle = multiConsumer.Connect(Bus))
            {
                Bus.Publish(new PingMessage());

                received.Select().Any().ShouldBe(true);
            }
        }

        [Test]
        public void Should_show_that_the_specified_type_was_received()
        {
            var consumer = new PingPongConsumer(TestTimeout);

            using (ConnectHandle handle = consumer.Connect(Bus))
            {
                var pingMessage = new PingMessage();
                Bus.Publish(pingMessage);
                Bus.Publish(new PongMessage(pingMessage.CorrelationId));

                consumer.Received.Select<PingMessage>().Any().ShouldBe(true);
                consumer.Received.Select<PongMessage>(received => received.Context.Message.CorrelationId == pingMessage.CorrelationId).Any().ShouldBe(true);
            }
        }


        class PingPongConsumer :
            MultiTestConsumer
        {
            public PingPongConsumer(TimeSpan timeout)
                : base(timeout)
            {
                Consume<PingMessage>();
                Consume<PongMessage>();
            }
        }
    }
}