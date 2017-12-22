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


    [TestFixture]
    public class PublisherConfirm_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_call_the_ack_method_upon_delivery()
        {
            await InputQueueSendEndpoint.Send(new A
            {
                StringA = "ValueA",
            });

            await _received;
        }

        Task<ConsumeContext<A>> _received;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _received = Handled<A>(configurator);
        }


        class A
        {
            public string StringA { get; set; }
        }
    }
}