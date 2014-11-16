// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq.Tests
{
    namespace Send_Specs
    {
        using System;
        using System.Threading.Tasks;
        using EndpointConfigurators;
        using NUnit.Framework;


        [TestFixture]
        public class When_a_message_is_send_to_the_local_endpoint :
            LocalRabbitMqTestFixture
        {
            [Test]
            public async void Should_be_received()
            {
                ISendEndpoint endpoint = await LocalBus.GetSendEndpoint(LocalBusUri);

                var message = new A {Id = Guid.NewGuid()};
                await endpoint.Send(message);

                A received = await _receivedA;

                Assert.AreEqual(message.Id, received.Id);
            }

            Task<A> _receivedA;

            protected override void ConfigureLocalReceiveEndpoint(IReceiveEndpointConfigurator configurator)
            {
                base.ConfigureLocalReceiveEndpoint(configurator);

                _receivedA = Handler<A>(configurator);
            }
        }


        class A
        {
            public Guid Id { get; set; }
        }
    }
}