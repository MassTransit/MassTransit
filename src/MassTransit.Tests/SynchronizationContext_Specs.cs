// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Windows.Forms;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using Messages;
    using NUnit.Framework;
    using TestFramework;
    using TextFixtures;

    [TestFixture, Explicit("This is clearly difficult to test")]
    public class SynchronizationContext_Specs :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_work_in_non_default_synchronization_context()
        {
            var synchronizationContext = new WindowsFormsSynchronizationContext();

            SynchronizationContext.SetSynchronizationContext(synchronizationContext);

            var pongReceived = new FutureMessage<PongMessage>();
            var pingReceived = new FutureMessage<PingMessage>();

            RemoteBus.SubscribeContextHandler<PingMessage>(x =>
                {
                    pingReceived.Set(x.Message);
                    x.Respond(new PongMessage {CorrelationId = x.Message.CorrelationId});
                });
            LocalBus.ShouldHaveSubscriptionFor<PingMessage>();

            var ping = new PingMessage();

            TimeSpan timeout = 8.Seconds();

            IAsyncResult request = LocalBus.BeginPublishRequest(ping, null, null, x =>
                {
                    x.Handle<PongMessage>(message =>
                        {
                            message.CorrelationId.ShouldEqual(ping.CorrelationId,
                                "The response correlationId did not match");
                            pongReceived.Set(message);
                        });

                    x.SetTimeout(timeout);
                });

            synchronizationContext.Wait(new[] {request.AsyncWaitHandle.Handle}, true, 10000);

            LocalBus.EndPublishRequest<PingMessage>(request);

            pingReceived.IsAvailable(timeout).ShouldBeTrue("The ping was not received");
            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");
        }
    }
}