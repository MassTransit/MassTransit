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
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using MassTransit.Testing.MessageObservers;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_the_multi_test_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_distinguish_multiple_events()
        {
            var consumer = new PingPongConsumer(TestTimeout);

            var handle = Bus.ConnectReceiveEndpoint("boring", x => consumer.Configure(x));
            await handle.Ready;
            try
            {
                var pingMessage = new PingMessage();
                var pingMessage2 = new PingMessage();
                await Bus.Publish(pingMessage);
                await Bus.Publish(pingMessage2);

                Assert.IsTrue(consumer.Received.Select<PingMessage>(received => received.Context.Message.CorrelationId == pingMessage.CorrelationId).Any());
                Assert.IsTrue(consumer.Received.Select<PingMessage>(received => received.Context.Message.CorrelationId == pingMessage2.CorrelationId).Any());
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        public async Task Should_show_that_the_message_was_received_by_the_consumer()
        {
            var multiConsumer = new MultiTestConsumer(TestTimeout);
            ReceivedMessageList<PingMessage> received = multiConsumer.Consume<PingMessage>();

            var handle = Bus.ConnectReceiveEndpoint("boring2", x => multiConsumer.Configure(x));
            await handle.Ready;

            try
            {
                await Bus.Publish(new PingMessage());

                Assert.IsTrue(received.Select().Any());
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        public async Task Should_show_that_the_specified_type_was_received()
        {
            var consumer = new PingPongConsumer(TestTimeout);

            var handle = Bus.ConnectReceiveEndpoint("boring3", x => consumer.Configure(x));
            await handle.Ready;

            try
            {
                var pingMessage = new PingMessage();
                await Bus.Publish(pingMessage);
                await Bus.Publish(new PongMessage(pingMessage.CorrelationId));

                Assert.IsTrue(consumer.Received.Select<PingMessage>().Any());
                Assert.IsTrue(consumer.Received.Select<PongMessage>(received => received.Context.Message.CorrelationId == pingMessage.CorrelationId).Any());
            }
            finally
            {
                await handle.StopAsync();
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
