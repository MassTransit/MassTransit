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
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_message_in_a_consumer :
        InMemoryTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handler<PingMessage>(configurator, context => context.Publish(new PongMessage(context.Message.CorrelationId)));
        }

        [Test]
        public async Task Should_source_address_from_the_endpoint()
        {
            var responseHandled = ConnectPublishHandler<PongMessage>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            var context = await _handled;

            var responseContext = await responseHandled;

            responseContext.SourceAddress.ShouldBe(InputQueueAddress);
        }
    }
}