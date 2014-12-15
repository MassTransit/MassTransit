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
namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using EndpointConfigurators;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Sending_a_message_to_a_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async void Should_be_received()
        {
            await _requestClient.Request(new PingMessage());
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;

        [TestFixtureSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<Consumer>()
                .UseLog(Console.Out, async context => string.Format("Consumer: {0}", TypeMetadataCache<Consumer>.ShortName));
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }
    }
}