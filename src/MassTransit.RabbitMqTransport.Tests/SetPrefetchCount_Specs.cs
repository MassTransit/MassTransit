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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Util;


    [TestFixture]
    public class Using_a_concurrency_limit_on_a_receive_endpoint :
        RabbitMqTestFixture
    {
        IManagementEndpointConfigurator _management;
        TestConsumer _consumer;

        protected override void ConfigureBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            _management = configurator.ManagementEndpoint(host);
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInputQueueEndpoint(configurator);

            configurator.ConnectManagementEndpoint(_management);

            configurator.UseConcurrencyLimit(32, _management);

            _consumer = new TestConsumer(TestTimeout);

            _consumer.Configure(configurator);
        }


        class TestConsumer :
            MultiTestConsumer
        {
            public TestConsumer(TimeSpan timeout)
                : base(timeout)
            {
                Consume<A>();
            }
        }

        [Test]
        public async Task Should_allow_reconfiguration()
        {
            IRequestClient<SetConcurrencyLimit, ConcurrencyLimitUpdated> client = new PublishRequestClient<SetConcurrencyLimit, ConcurrencyLimitUpdated>(Bus,
                TestTimeout);

            SetConcurrencyLimit request = TypeMetadataCache<SetConcurrencyLimit>.InitializeFromObject(new
            {
                ConcurrencyLimit = 16,
                Timestamp = DateTime.UtcNow
            });

            var concurrencyLimitUpdated = await client.Request(request, TestCancellationToken);

            Assert.AreEqual(request.ConcurrencyLimit, concurrencyLimitUpdated.ConcurrencyLimit);
        }

        [Test, Explicit]
        public async Task Should_allow_reconfiguration_of_prefetch_count()
        {
            IRequestClient<SetPrefetchCount, PrefetchCountUpdated> client = new PublishRequestClient<SetPrefetchCount, PrefetchCountUpdated>(Bus,
                TestTimeout);

            for (int i = 0; i < 500; i++)
            {
                await Bus.Publish(new A());

                await Task.Delay(50);
            }

            SetPrefetchCount request = TypeMetadataCache<SetPrefetchCount>.InitializeFromObject(new
            {
                PrefetchCount = (ushort)32,
                Timestamp = DateTime.UtcNow,
                QueueName = "input_queue",
            });

            await client.Request(request, TestCancellationToken);

            for (int i = 0; i < 500; i++)
            {
                await Bus.Publish(new A());

                await Task.Delay(50);
            }

            Assert.IsTrue(_consumer.Received.Select<A>().Any());
        }

        class A { }
    }
}