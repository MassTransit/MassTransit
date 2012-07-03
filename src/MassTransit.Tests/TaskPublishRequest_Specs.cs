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
    using System.Threading.Tasks;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using RequestResponse;
    using TextFixtures;

#if NET40
    [TestFixture]
    public class Publishing_request_using_the_task_parallel_library :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_complete_all_related_tasks()
        {
            var pongReceived = new FutureMessage<PongMessage>();
            var continueCalled = new FutureMessage<Task<PongMessage>>();

            TimeSpan timeout = 8.Seconds();

            var ping = new PingMessage();
            ITaskRequest<PingMessage> request = LocalBus.PublishRequestAsync(ping, x =>
                {
                    x.SetTimeout(timeout);

                    x.Handle<PongMessage>(pongReceived.Set)
                        .ContinueWith(continueCalled.Set);
                });

            pongReceived.IsAvailable(timeout).ShouldBeTrue("The pong was not received");

            request.Task.Wait(timeout).ShouldBeTrue("Task was not completed");

            continueCalled.IsAvailable(timeout).ShouldBeTrue("The continuation was not called");
        }

        FutureMessage<PingMessage> _pingReceived;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _pingReceived = new FutureMessage<PingMessage>();
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Subscribe(x =>
                {
                    x.Handler<PingMessage>((context, message) =>
                        {
                            _pingReceived.Set(message);
                            context.Respond(new PongMessage {TransactionId = message.TransactionId});
                        });
                });
        }

        class PingMessage
        {
            public Guid TransactionId { get; set; }
        }

        class PongMessage
        {
            public Guid TransactionId { get; set; }
        }
    }
#endif
}