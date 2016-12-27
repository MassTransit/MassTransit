// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.HttpTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_an_endpoint :
        HttpTestFixture
    {
        [Test]
        public async Task Should_have_a_redelivery_flag_of_false()
        {
            ConsumeContext<PingMessage> context = await _handler;

            Assert.IsFalse(context.ReceiveContext.Redelivered);
        }

        [Test]
        public async Task Should_succeed()
        {
            await _handler;
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await RootEndpoint.Send(new PingMessage());
        }
    }


    [TestFixture]
    public class Sending_a_request_using_the_request_client :
        HttpTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            var message = await _response;

            Assert.AreEqual(message.CorrelationId, _ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, HostAddress, TestTimeout);

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureHttpReceiveEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, cxt => cxt.RespondAsync(new PongMessage(cxt.Message.CorrelationId)));
        }
    }
}