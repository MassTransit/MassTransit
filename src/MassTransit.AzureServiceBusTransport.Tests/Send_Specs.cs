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
namespace MassTransit.AzureServiceBusTransport.Tests
{
    using System.Threading.Tasks;
    using Configuration;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_an_endpoint :
        AzureServiceBusTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
        }

        [Test]
        public async Task Should_have_a_redelivery_flag_of_false()
        {
            var context = await _handler;

            Assert.IsFalse(context.ReceiveContext.Redelivered);
        }

        [Test]
        public async Task Should_succeed()
        {
            await _handler;
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_request_client :
        AzureServiceBusTestFixture
    {
        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TestTimeout);

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }

        [Test]
        public async Task Should_receive_the_response()
        {
            PongMessage message = await _response;

            Assert.AreEqual(message.CorrelationId, _ping.Result.Message.CorrelationId);
        }
    }

    [TestFixture]
    public class Sending_a_request_using_the_request_client_with_response_endpoint :
        AzureServiceBusTestFixture
    {
        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _requestClient = await Host.CreateRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TestTimeout);

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }

        [Test]
        public async Task Should_receive_the_response()
        {
            PongMessage message = await _response;

            Assert.AreEqual(message.CorrelationId, _ping.Result.Message.CorrelationId);
        }
    }
}