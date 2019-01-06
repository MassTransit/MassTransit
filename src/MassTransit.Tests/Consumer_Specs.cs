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
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Sending_a_message_to_a_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await _requestClient.Request(new PingMessage());
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage, PongMessage>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<Consumer>(
                x => x.UseConsoleLog(async log => string.Format("{1:u} {2:F0} Consumer: {0}", TypeMetadataCache<Consumer>.ShortName,
                    log.StartTime, log.Duration.TotalMilliseconds)));
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
