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
namespace MassTransit.Tests.Performance
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture, Explicit]
    public class Performance_of_the_in_memory_transport :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_wicked_fast()
        {
            int limit = 50000;
            int count = 0;

            await _requestClient.Request(new PingMessage());

            Stopwatch timer = Stopwatch.StartNew();

            await Task.WhenAll(Enumerable.Range(0, limit).Select(async x =>
            {
                await _requestClient.Request(new PingMessage());

                Interlocked.Increment(ref count);
            }));

            timer.Stop();

            Console.WriteLine("Time to process {0} messages = {1}", count, timer.ElapsedMilliseconds + "ms");
            Console.WriteLine("Messages per second: {0}", count * 1000 / timer.ElapsedMilliseconds);
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseBsonSerializer();

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<PingMessage>(async context =>
            {
                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            });
        }
    }


    [TestFixture, Explicit]
    public class Performance_of_dynamic_interface_implementations :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_also_really_fast()
        {
            int limit = 50000;
            int count = 0;

            await _requestClient.Request(new PerformanceRequestImpl());

            Stopwatch timer = Stopwatch.StartNew();

            await Task.WhenAll(Enumerable.Range(0, limit).Select(async x =>
            {
                await _requestClient.Request(new PerformanceRequestImpl());

                Interlocked.Increment(ref count);
            }));

            timer.Stop();

            Console.WriteLine("Time to process {0} messages = {1}", count, timer.ElapsedMilliseconds + "ms");
            Console.WriteLine("Messages per second: {0}", count * 1000 / timer.ElapsedMilliseconds);
        }

        IRequestClient<PerformanceRequest, PerformanceResult> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PerformanceRequest, PerformanceResult>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<PerformanceRequest>(async context =>
            {
                await context.RespondAsync(new PerformanceResultImpl(context.Message.Id));
            });
        }


        public interface PerformanceResult
        {
            Guid Id { get; }
        }


        class PerformanceResultImpl :
            PerformanceResult
        {
            public PerformanceResultImpl(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; private set; }
        }


        public interface PerformanceRequest
        {
            Guid Id { get; }
        }


        class PerformanceRequestImpl :
            PerformanceRequest
        {
            public PerformanceRequestImpl()
            {
                Id = Guid.NewGuid();
            }

            public Guid Id { get; private set; }
        }
    }
}