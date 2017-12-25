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
    using GreenPipes;
    using GreenPipes.Introspection;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_a_queue :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_an_empty_fault_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.FaultAddress.ShouldBe(null);
        }

        [Test]
        public async Task Should_have_an_empty_response_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.ResponseAddress.ShouldBe(null);
        }

        [Test]
        public async Task Should_include_the_correlation_id()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.CorrelationId.ShouldBe(_correlationId);
        }

        [Test]
        public async Task Should_include_the_destination_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.DestinationAddress.ShouldBe(InputQueueAddress);
        }

        [Test]
        public async Task Should_include_the_header()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            object header;
            ping.Headers.TryGetHeader("One", out header);
            header.ShouldBe("1");
        }

        [Test]
        public async Task Should_include_the_source_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.SourceAddress.ShouldBe(BusAddress);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Guid _correlationId;

        [OneTimeSetUp]
        public void Setup()
        {
            _correlationId = Guid.NewGuid();

            InputQueueSendEndpoint.Send(new PingMessage(), Pipe.New<SendContext<PingMessage>>(x => x.UseExecute(context =>
            {
                context.CorrelationId = _correlationId;
                context.Headers.Set("One", "1");
            })))
                .Wait(TestCancellationToken);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_queue :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_received_the_response_on_the_handler()
        {
            PongMessage message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async Task Should_have_the_matching_correlation_id()
        {
            ConsumeContext<PongMessage> context = await _responseHandler;

            context.Message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async Task Should_include_the_destination_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.DestinationAddress.ShouldBe(InputQueueAddress);
        }

        [Test]
        public async Task Should_include_the_response_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.ResponseAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_include_the_source_address()
        {
            ConsumeContext<PingMessage> ping = await _ping;

            ping.SourceAddress.ShouldBe(BusAddress);
        }

        [Test]
        public async Task Should_receive_the_response()
        {
            ConsumeContext<PongMessage> context = await _responseHandler;

            context.ConversationId.ShouldBe(_conversationId);
        }

        [Test]
        public async Task Should_call_the_middleware()
        {
            ConsumeContext<PongMessage> context = await _responseMiddleware.Task;

            context.ConversationId.ShouldBe(_conversationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<ConsumeContext<PongMessage>> _responseHandler;
        Task<Request<PingMessage>> _request;
        Task<PongMessage> _response;
        Guid? _conversationId;
        TaskCompletionSource<ConsumeContext<PongMessage>> _responseMiddleware;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _responseHandler = SubscribeHandler<PongMessage>();
            _conversationId = NewId.NextGuid();

            _responseMiddleware = GetTask<ConsumeContext<PongMessage>>();

            _request = Bus.Request(InputQueueAddress, new PingMessage(), x =>
            {
                x.ConversationId = _conversationId;

                _response = x.Handle<PongMessage>(async context =>
                {
                    Console.WriteLine("Response received");
                }, cfg => cfg.UseExecute(context => _responseMiddleware.TrySetResult(context)));
                x.Timeout = TestTimeout;
            });

            await _request;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_with_two_handlers :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_received_the_actual_response()
        {
            PingNotSupported message = await _notSupported;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async Task Should_not_complete_the_handler()
        {
            await _notSupported;

            await BusSendEndpoint.Send(new PongMessage((await _ping).Message.CorrelationId));

            Assert.That(async () => await _response, Throws.TypeOf<TaskCanceledException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<ConsumeContext<PongMessage>> _responseHandler;
        Task<Request<PingMessage>> _request;
        Task<PongMessage> _response;
        Task<PingNotSupported> _notSupported;

        [OneTimeSetUp]
        public async Task Setup()
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

            await _request;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PingNotSupported(x.Message.CorrelationId)));
        }
    }

    [TestFixture]
    public class Publishing_a_request:
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_received_the_response_on_the_handler()
        {
            PongMessage message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        [Test, Explicit]
        public async Task Should_wonderful_display()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());

        }


        Task<ConsumeContext<PingMessage>> _ping;
        Task<ConsumeContext<PongMessage>> _responseHandler;
        Task<Request<PingMessage>> _request;
        Task<PongMessage> _response;
        Task<PingNotSupported> _notSupported;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _responseHandler = SubscribeHandler<PongMessage>();


            _request = Bus.PublishRequest(new PingMessage(), x =>
            {
                x.Timeout = TimeSpan.FromSeconds(10);
                _response = x.Handle<PongMessage>(async _ =>
                {
                });

            });
            await _request;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_with_no_handler :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_a_request_timeout_exception_on_the_handler()
        {
            Assert.That(async () => await _response, Throws.TypeOf<RequestTimeoutException>());
        }

        [Test]
        public async Task Should_receive_a_request_timeout_exception_on_the_request()
        {
            Assert.That(async () =>
            {
                Request<PingMessage> request = await _request;

                await request.Task;
            }, 
            Throws.TypeOf<RequestTimeoutException>());
        }

        Task<Request<PingMessage>> _request;
        Task<PongMessage> _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _request = Bus.Request(InputQueueAddress, new PingMessage(), x =>
            {
                x.Timeout = TimeSpan.FromSeconds(1);

                _response = x.Handle<PongMessage>(async _ =>
                {
                });
            });
            await _request;
        }
    }
}