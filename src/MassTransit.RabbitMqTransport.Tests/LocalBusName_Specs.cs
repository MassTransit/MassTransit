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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class LocalBusName_Specs :
        RabbitMqTestFixture
    {

        [Test]
        public async Task Should_get_the_response_to_the_bus()
        {
            IRequestClient<PingMessage,PongMessage> client = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TestTimeout);

            await client.Request(new PingMessage(), TestCancellationToken);
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureRabbitMqBusHost(configurator, host);

            configurator.OverrideDefaultBusEndpointQueueName($"super-bus-{NewId.NextGuid():N}");
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            configurator.Handler<PingMessage>(context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }
    }
}