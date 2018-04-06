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
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_messages_through_the_outbox :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_receive_the_response()
        {
            Task<ConsumeContext<PongMessage>> responseHandler = SubscribeHandler<PongMessage>();

            Assert.That(async () =>
                {
                    var response = await Bus.Request<PingMessage, PongMessage>(InputQueueAddress, new PingMessage(), TestCancellationToken,
                        RequestTimeout.After(s: 3));
                },
                Throws.TypeOf<RequestFaultException>());

            await _pingReceived.Task;

            Console.WriteLine("Ping was received");

            Assert.That(
                async () => await responseHandler.WithCancellation(new CancellationTokenSource(300).Token),
                Throws.TypeOf<TaskCanceledException>());
        }

        TaskCompletionSource<ConsumeContext<PingMessage>> _pingReceived;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseInMemoryOutbox();

            _pingReceived = GetTask<ConsumeContext<PingMessage>>();

            configurator.Handler<PingMessage>(context =>
            {
                _pingReceived.TrySetResult(context);

                context.Respond(new PongMessage(context.Message.CorrelationId));

                throw new IntentionalTestException("This time bad things happen");
            });
        }
    }
}