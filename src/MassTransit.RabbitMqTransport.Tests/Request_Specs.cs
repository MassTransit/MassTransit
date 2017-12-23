// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_using_the_request_client :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        public Sending_a_request_using_the_request_client()
        {
            RabbitMqTestHarness.OnConfigureRabbitMqHost += ConfigureHost;
        }

        void ConfigureHost(IRabbitMqHostConfigurator configurator)
        {
            configurator.PublisherConfirmation = false;
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8), context => context.SetAwaitAck(false));

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }

    [TestFixture]
    public class Sending_a_request_using_the_request_client_in_a_consumer :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        [Test]
        public async Task Should_have_the_conversation_id()
        {
            var ping = await _ping;
            var a = await _a;

            ping.ConversationId.ShouldBe(a.ConversationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClientFactory<A, B> _factory;
        IRequestClient<PingMessage, PongMessage> _requestClient;
        Task<ConsumeContext<A>> _a;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _factory = await Host.CreateRequestClientFactory<A, B>(InputQueueAddress, TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8));

            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8));

            _response = _requestClient.Request(new PingMessage());
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _factory.DisposeAsync();
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                var client = _factory.CreateRequestClient(x);
                await client.Request(new A(), x.CancellationToken);

                x.Respond(new PongMessage(x.Message.CorrelationId));
            });

            _a = Handler<A>(configurator, x => x.RespondAsync(new B()));
        }


        class A
        {
        }


        class B
        {
            
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_request_client_with_no_confirmations :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            var message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8));

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_missing_service :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_timeout()
        {
            Assert.That(async () => await _response, Throws.TypeOf<RequestTimeoutException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(1));

            _response = _requestClient.Request(new PingMessage());
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_faulty_service :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_receive_the_exception()
        {
            Assert.That(async () => await _response, Throws.TypeOf<RequestFaultException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(8));

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                throw new InvalidOperationException("This is an expected test failure");
            });
        }
    }
}