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
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_a_simple_connection_to_rabbit :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_be_good()
        {
        }

        [Test]
        public async Task Should_receive_a_message()
        {
            var response = SubscribeHandler<PongMessage>();

            await InputQueueSendEndpoint.Send(new PingMessage(), x =>
            {
                x.ResponseAddress = Bus.Address;
            });

            await response;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }
    }

    [TestFixture]
    public class Using_a_simple_connection_to_rabbit_to_publish :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_be_good()
        {
        }

        [Test]
        public async Task Should_receive_a_message()
        {
            var response = SubscribeHandler<PongMessage>();

            await Bus.Publish(new PingMessage(), x =>
            {
                x.ResponseAddress = Bus.Address;
            });

            await response;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }
    }

    [TestFixture]
    public class Publishing_without_a_listener :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_a_message()
        {
            await Bus.Publish(new PingMessage(), x =>
            {
                x.ResponseAddress = Bus.Address;
            });
        }
    }
}