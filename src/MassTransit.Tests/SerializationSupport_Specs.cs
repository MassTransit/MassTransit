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
namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_using_mixed_serialization_types :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_able_to_read_xml_when_using_json()
        {
            _responseReceived = ConnectPublishHandler<B>();

            await InputQueueSendEndpoint.Send(new A {Key = "Hello"});

            await _requestReceived;

            await _responseReceived;
        }

        Task<ConsumeContext<A>> _requestReceived;
        Task<ConsumeContext<B>> _responseReceived;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseXmlSerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            // TODO would be nice to support serialization per receiving endpoint

            // configurator.UseJsonSerializer();

            _requestReceived = Handler<A>(configurator, context => context.RespondAsync(new B()));
        }


        class A
        {
            public string Key { get; set; }
        }


        class B
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}