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
    using System;
    using System.Threading.Tasks;
    using EndpointConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using TestConsumers;
    using TestFramework;
    using TestFramework.Messages;
    using TextFixtures;


    [TestFixture]
    public class Sending_a_message_to_a_queue :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_have_an_empty_fault_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.FaultAddress.ShouldBeNull();
        }

        [Test]
        public async void Should_have_an_empty_response_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.ResponseAddress.ShouldBeNull();
        }

        [Test]
        public async void Should_include_the_destination_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.DestinationAddress.ShouldEqual(InputQueueAddress);
        }

        [Test]
        public async void Should_include_the_source_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.SourceAddress.ShouldEqual(BusAddress);
        }

        Task<ConsumeContext<PingMessage>> _ping;

        [TestFixtureSetUp]
        public void Setup()
        {
            InputQueueSendEndpoint.Send(new PingMessage())
                .Wait(TestCancellationToken);
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_queue :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_have_received_the_response_on_the_handler()
        {
            PongMessage message = await _response;

            message.CorrelationId.ShouldEqual(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async void Should_have_the_matching_correlation_id()
        {
            ConsumeContext<PongMessage> context = await _responseHandler;

            context.Message.CorrelationId.ShouldEqual(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async void Should_include_the_destination_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.DestinationAddress.ShouldEqual(InputQueueAddress);
        }

        [Test]
        public async void Should_include_the_response_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.ResponseAddress.ShouldEqual(BusAddress);
        }

        [Test]
        public async void Should_include_the_source_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.SourceAddress.ShouldEqual(BusAddress);
        }

        [Test]
        public async void Should_receive_the_response()
        {
            ConsumeContext<PongMessage> context = await _responseHandler;
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<ConsumeContext<PongMessage>> _responseHandler;
        Task<Request<PingMessage>> _request;
        Task<PongMessage> _response;

        [TestFixtureSetUp]
        public void Setup()
        {
            _responseHandler = SubscribeHandler<PongMessage>();

            _request = Bus.Request(InputQueueAddress, new PingMessage(), x =>
            {
                _response = x.Handle<PongMessage>(async _ =>
                {
                });
            });
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }

    [TestFixture]
    public class Sending_a_request_with_two_handlers :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_have_received_the_actual_response()
        {
            PingNotSupported message = await _notSupported;

            message.CorrelationId.ShouldEqual(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async void Should_not_complete_the_handler()
        {
            await _notSupported;

            await BusSendEndpoint.Send(new PongMessage((await _ping).Message.CorrelationId));

            Assert.Throws<TaskCanceledException>(async () =>
            {
                await _response;
            });
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<ConsumeContext<PongMessage>> _responseHandler;
        Task<Request<PingMessage>> _request;
        Task<PongMessage> _response;
        Task<PingNotSupported> _notSupported;

        [TestFixtureSetUp]
        public void Setup()
        {
            _responseHandler = SubscribeHandler<PongMessage>();

            _request = Bus.Request(InputQueueAddress, new PingMessage(), x =>
            {
                _response = x.Handle<PongMessage>(async _ =>
                {
                });

                _notSupported = x.Handle<PingNotSupported>(async _ =>
                {
                });
            });
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PingNotSupported(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_with_no_handler :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_receive_a_request_timeout_exception_on_the_handler()
        {
            Assert.Throws<RequestTimeoutException>(async () => await _response);
        }

        [Test]
        public async void Should_receive_a_request_timeout_exception_on_the_request()
        {
            Assert.Throws<RequestTimeoutException>(async () =>
            {
                var request = await _request;

                await request.Task;
            });
        }

        Task<Request<PingMessage>> _request;
        Task<PongMessage> _response;

        [TestFixtureSetUp]
        public void Setup()
        {
            _request = Bus.Request(InputQueueAddress, new PingMessage(), x =>
            {
                x.Timeout = 1.Seconds();

                _response = x.Handle<PongMessage>(async _ =>
                {
                });
            });
        }
    }


    [TestFixture]
    public class MessageContext_Specs :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void A_random_header_should_pass()
        {
            Guid id = Guid.NewGuid();

            var received = new FutureMessage<PingMessage>();

            LocalBus.SubscribeHandler<PingMessage>(message =>
            {
                Assert.AreEqual(id.ToString(), LocalBus.Context().Headers["RequestId"]);

                received.Set(message);
            });

            LocalBus.Publish(new PingMessage(), context => context.SetHeader("RequestId", id.ToString()));

            Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
        }

        [Test]
        public void A_response_should_be_published_if_no_reply_address_is_specified()
        {
            var ping = new PingMessage();

            var otherConsumer = new TestMessageConsumer<PongMessage>();
            RemoteBus.SubscribeInstance(otherConsumer);

            LocalBus.ShouldHaveRemoteSubscriptionFor<PongMessage>();

            var consumer = new TestCorrelatedConsumer<PongMessage, Guid>(ping.CorrelationId);
            LocalBus.SubscribeInstance(consumer);

            var pong = new FutureMessage<PongMessage>();

            RemoteBus.SubscribeHandler<PingMessage>(message =>
            {
                pong.Set(new PongMessage(message.CorrelationId));

                RemoteBus.Context().Respond(pong.Message);
            });

            RemoteBus.ShouldHaveRemoteSubscriptionFor<PongMessage>();
            LocalBus.ShouldHaveRemoteSubscriptionFor<PingMessage>();

            LocalBus.Publish(ping);

            pong.IsAvailable(8.Seconds()).ShouldBeTrue("No pong generated");

            consumer.ShouldHaveReceivedMessage(pong.Message, 8.Seconds());
            otherConsumer.ShouldHaveReceivedMessage(pong.Message, 8.Seconds());
        }

        [Test]
        public void The_conversation_id_should_pass()
        {
            Guid id = Guid.NewGuid();

            var received = new FutureMessage<PingMessage>();

            LocalBus.SubscribeHandler<PingMessage>(message =>
            {
                Assert.AreEqual(id.ToString(), LocalBus.Context().ConversationId);

                received.Set(message);
            });

            LocalBus.Publish(new PingMessage(), context => context.SetConversationId(id.ToString()));

            Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
        }

        [Test]
        public void The_correlation_id_should_pass()
        {
            Guid id = Guid.NewGuid();

            var received = new FutureMessage<PingMessage>();

            LocalBus.SubscribeHandler<PingMessage>(message =>
            {
                Assert.AreEqual(id.ToString(), LocalBus.Context().CorrelationId);

                received.Set(message);
            });

            LocalBus.Publish(new PingMessage(), context => context.SetCorrelationId(id.ToString()));

            Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
        }

        [Test]
        public void The_fault_address_should_pass()
        {
            var received = new FutureMessage<PingMessage>();

            LocalBus.SubscribeHandler<PingMessage>(message =>
            {
                Assert.AreEqual(LocalBus.Endpoint.Address.Uri, LocalBus.Context().FaultAddress);

                received.Set(message);
            });

            LocalBus.Publish(new PingMessage(), context => context.SendFaultTo(LocalBus));

            Assert.IsTrue(received.IsAvailable(5.Seconds()), "No message was received");
        }
    }
}