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
namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_to_another_scope :
        TwoScopeAzureServiceBusTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handler;
        IRequestClient<PingMessage> _requestClient;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handler<PingMessage>(configurator, context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }

        protected override void ConfigureSecondInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = SecondBus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
        }

        [Test]
        public async Task Should_succeed()
        {
            var pingMessage = new PingMessage();

            var response = await _requestClient.GetResponse<PongMessage>(pingMessage);

            Assert.That(response.Message.CorrelationId, Is.EqualTo(pingMessage.CorrelationId));
        }
    }
}
