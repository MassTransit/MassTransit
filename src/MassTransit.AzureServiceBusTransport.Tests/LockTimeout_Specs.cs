// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture, Explicit]
    public class Renewing_a_lock_on_an_existing_message :
        AzureServiceBusTestFixture
    {
        public Renewing_a_lock_on_an_existing_message()
        {
            TestTimeout = TimeSpan.FromMinutes(3);
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.LockDuration = TimeSpan.FromSeconds(60);

            configurator.UseRenewLock(TimeSpan.FromSeconds(20));

            configurator.Consumer<PingConsumer>();
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());
        }

        [Test]
        public async Task Should_complete_the_consumer()
        {
            var context = await PingConsumer.Completed.Task;
        }

        class PingConsumer :
            IConsumer<PingMessage>
        {
            public static TaskCompletionSource<PingMessage> Completed = new TaskCompletionSource<PingMessage>();

            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await Task.Delay(TimeSpan.FromMinutes(2), context.CancellationToken).ConfigureAwait(false);

                Completed.TrySetResult(context.Message);

            }
        }
    }
}