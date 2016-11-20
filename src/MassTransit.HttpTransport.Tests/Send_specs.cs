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
    using System;
    using System.Threading.Tasks;
    using Builders;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework.Messages;
    using TestFramework;


    [TestFixture]
    public class Sending_a_message_to_an_endpoint :
       HttpBusTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handler;
        
        protected override void ConfigureInputQueueEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            Await(() => InputQueueSendEndpoint.Send(new PingMessage()));
        }

        [Test]
        public async void Should_have_a_redelivery_flag_of_false()
        {
            var context = await _handler;

            Assert.IsFalse(context.ReceiveContext.Redelivered);
        }

        [Test]
        public async void Should_succeed()
        {
            await _handler;
        }
    }

    [TestFixture]
    public class Sending_a_request_using_the_request_client :
        HttpBusTestFixture
    {       
        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [TestFixtureSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TestTimeout);

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureInputQueueEndpoint(IHttpReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async cxt => await cxt.RespondAsync(new PongMessage(cxt.Message.CorrelationId)));
        }   

        [Test]
        public async void Should_receive_the_response()
        {
            var re = Bus.GetProbeResult();
            Console.WriteLine(re.ToJsonString());
            PongMessage message = await _response;

            Assert.AreEqual(message.CorrelationId, _ping.Result.Message.CorrelationId);
        }
    }
}