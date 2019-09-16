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
namespace MassTransit.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Util;


    [TestFixture]
    public class Sending_a_message_to_a_observer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await _requestClient.Request(new PingMessage());
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;
        PingObserver _observer;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage, PongMessage>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _observer = new PingObserver();
            configurator.Observer(_observer, x =>
                x.UseLog(Console.Out, async logContext => string.Format("Observer: {0}", TypeMetadataCache<PingObserver>.ShortName)));
        }


        class PingObserver :
            IObserver<ConsumeContext<PingMessage>>
        {
            readonly ConcurrentBag<PingMessage> _received;

            public PingObserver()
            {
                _received = new ConcurrentBag<PingMessage>();
            }

            public void OnNext(ConsumeContext<PingMessage> context)
            {
                _received.Add(context.Message);

                context.Respond(new PongMessage(context.Message.CorrelationId));
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
            }
        }
    }
}