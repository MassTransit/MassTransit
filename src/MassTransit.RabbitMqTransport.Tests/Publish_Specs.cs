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
    namespace Send_Specs
    {
        using System;
        using System.Threading.Tasks;
        using Configuration;
        using NUnit.Framework;


        [TestFixture]
        public class WhenAMessageIsSendToTheEndpoint :
            RabbitMqTestFixture
        {
            [Test]
            public async void Should_be_received()
            {
                ISendEndpoint endpoint = await Bus.GetSendEndpoint(InputQueueAddress);

                var message = new A {Id = Guid.NewGuid()};
                await endpoint.Send(message);

                ConsumeContext<A> received = await _receivedA;

                Assert.AreEqual(message.Id, received.Message.Id);
            }

            Task<ConsumeContext<A>> _receivedA;

            protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureInputQueueEndpoint(configurator);

                _receivedA = Handler<A>(configurator);
            }
        }


        [TestFixture]
        public class WhenAMessageIsPublishedToTheEndpoint :
            RabbitMqTestFixture
        {
            [Test]
            public async void Should_be_received()
            {
                var message = new A {Id = Guid.NewGuid()};
                await Bus.Publish(message);

                ConsumeContext<A> received = await _receivedA;

                Assert.AreEqual(message.Id, received.Message.Id);
            }

            Task<ConsumeContext<A>> _receivedA;

            protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureInputQueueEndpoint(configurator);

                _receivedA = Handler<A>(configurator);
            }
        }


        class A
        {
            public Guid Id { get; set; }
        }
    }
}