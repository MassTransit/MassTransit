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
    using ConsumerBind_Specs;
    using NUnit.Framework;


    [TestFixture]
    public class Sending_to_a_publish_exchange :
        ConsumerBindingTestFixture
    {
        [Test]
        public async Task Should_arrive_on_the_receive_endpoint()
        {
            var sendSettings = _host.Settings.GetSendSettings(typeof(A));
            var sendAddress = _host.Settings.GetSendAddress(sendSettings);

            var endpoint = await Bus.GetSendEndpoint(sendAddress);

            await endpoint.Send(new A());

            await _handled;
        }

        Task<ConsumeContext<A>> _handled;
        IRabbitMqHost _host;

        protected override void ConfigureBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureBusHost(configurator, host);

            _host = host;
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }
    }
}