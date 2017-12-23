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
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Skipping_messages_should_not_crash_the_service :
        RabbitMqTestFixture
    {
        Task<ConsumeContext<PingMessage>> _pingsHandled;

        [Test, Explicit]
        public async Task Should_properly_complete_without_dying()
        {
            await Task.WhenAll(Enumerable.Range(0, 1000).Select(n => Bus.Publish(new PingMessage())));

            await _pingsHandled;

            await Task.Delay(10000);
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Bind<PingMessage>();
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.ReceiveEndpoint(host, "monitor", e =>
            {
                _pingsHandled = Handled<PingMessage>(e, 1000);
            });
        }
    }
}