// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.WebJobs.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using ServiceBusIntegration;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Creating_a_brokered_message_receiver
    {
        [Test]
        public async Task Should_create_the_brokered_message_receiver()
        {
            var message = new Mock<Message>();
            var logger = new Mock<ILogger>();

            var binder = new Mock<IBinder>();

            var handler = Bus.Factory.CreateBrokeredMessageReceiver(binder.Object, cfg =>
            {
                cfg.CancellationToken = CancellationToken.None;
                cfg.SetLog(logger.Object);
                cfg.InputAddress = new Uri("sb://masstransit-build.servicebus.windows.net/input-queue");

                cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
                cfg.Consumer(() => new Consumer());
            });

            Console.WriteLine(handler.GetProbeResult().ToJsonString());

//            await handler.Handle(message.Object);
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                TestContext.Out.WriteLine("Hello");

                return Task.CompletedTask;
            }
        }
    }
}
