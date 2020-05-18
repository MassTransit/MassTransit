// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
using NUnit.Framework;


namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;


    [TestFixture]
    public class When_an_entity_name_is_specified_on_the_message_type :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await Bus.Publish<CustomEntityMessage>(new
            {
                Value = "Yawn"
            });

            ConsumeContext<CustomEntityMessage> received = await _receivedA;

            Assert.That(received.DestinationAddress, Is.EqualTo(new Uri(HostAddress, EntityName)));
        }

        Task<ConsumeContext<CustomEntityMessage>> _receivedA;
        const string EntityName = "custom-message";

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Message<CustomEntityMessage>(x =>
            {
                x.SetEntityName(EntityName);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            _receivedA = Handled<CustomEntityMessage>(configurator);
        }


        public interface CustomEntityMessage
        {
            string Value { get; }
        }
    }
}