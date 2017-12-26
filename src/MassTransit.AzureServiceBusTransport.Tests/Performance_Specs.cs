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
namespace MassTransit.AzureServiceBusTransport.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture, Explicit]
    public class Performance_of_the_azure_service_bus_transport :
        AzureServiceBusTestFixture
    {
        IRequestClient<PingMessage, PongMessage> _requestClient;

        public Performance_of_the_azure_service_bus_transport()
        {
            TestTimeout = TimeSpan.FromMinutes(5);
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            base.ConfigureServiceBusBus(configurator);

            configurator.MaxConcurrentCalls = 16;
            configurator.PrefetchCount = 64;
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            configurator.PrefetchCount = 64;
            configurator.MaxConcurrentCalls = 16;

            configurator.Handler<PingMessage>(async context =>
            {
                try
                {
                    await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        [Test]
        public async Task Should_be_wicked_fast()
        {
            int limit = 2000;
            int count = 0;

            await _requestClient.Request(new PingMessage());

            Stopwatch timer = Stopwatch.StartNew();

            await Task.WhenAll(Enumerable.Range(0, limit).Select(async x =>
            {
                await _requestClient.Request(new PingMessage());

                Interlocked.Increment(ref count);
            }));

            timer.Stop();

            Console.WriteLine("Time to process {0} messages = {1}", count * 2, timer.ElapsedMilliseconds + "ms");
            Console.WriteLine("Messages per second: {0}", count * 2 * 1000 / timer.ElapsedMilliseconds);
        }
    }
}