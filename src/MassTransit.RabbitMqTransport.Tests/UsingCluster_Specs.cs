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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_clustering_nodes_into_a_logical_broker :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_use_the_logical_host_name()
        {
            var endpoint = await Bus.GetSendEndpoint(InputQueueAddress);

            var message = new PingMessage();
            await endpoint.Send(message);

            ConsumeContext<PingMessage> received = await _receivedA;

            Assert.AreEqual(message.CorrelationId, received.Message.CorrelationId);

            Assert.AreEqual(_logicalHostAddress.Host, received.DestinationAddress.Host);
        }

        public When_clustering_nodes_into_a_logical_broker()
            : base(new Uri("rabbitmq://cluster/test/"))
        {
        }

        readonly Uri _logicalHostAddress = new Uri("rabbitmq://cluster/test/");

        Task<ConsumeContext<PingMessage>> _receivedA;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _receivedA = Handled<PingMessage>(configurator);
        }
    }
}