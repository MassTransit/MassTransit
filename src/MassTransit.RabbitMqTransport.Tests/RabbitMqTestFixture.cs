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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using RabbitMqTransport.Testing;
    using RabbitMQ.Client;
    using TestFramework;
    using Transports;


    public class RabbitMqTestFixture :
        BusTestFixture
    {
        protected RabbitMqTestHarness RabbitMqTestHarness { get; }

        public RabbitMqTestFixture(Uri logicalHostAddress = null, string inputQueueName = null)
            : this(new RabbitMqTestHarness(inputQueueName), logicalHostAddress)
        {
        }

        public RabbitMqTestFixture(RabbitMqTestHarness harness, Uri logicalHostAddress = null)
            : base(harness)
        {
            RabbitMqTestHarness = harness;

            if (logicalHostAddress != null)
            {
                RabbitMqTestHarness.NodeHostName = RabbitMqTestHarness.HostAddress.Host;
                RabbitMqTestHarness.HostAddress = logicalHostAddress;
            }

            RabbitMqTestHarness.OnConfigureRabbitMqHost += ConfigureRabbitMqHost;
            RabbitMqTestHarness.OnConfigureRabbitMqBus += ConfigureRabbitMqBus;
            RabbitMqTestHarness.OnConfigureRabbitMqBusHost += ConfigureRabbitMqBusHost;
            RabbitMqTestHarness.OnConfigureRabbitMqReceiveEndpoint += ConfigureRabbitMqReceiveEndpoint;
            RabbitMqTestHarness.OnCleanupVirtualHost += OnCleanupVirtualHost;
        }


        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => RabbitMqTestHarness.InputQueueSendEndpoint;

        protected Uri InputQueueAddress => RabbitMqTestHarness.InputQueueAddress;

        protected Uri HostAddress => RabbitMqTestHarness.HostAddress;

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => RabbitMqTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => RabbitMqTestHarness.Sent;

        protected Uri BusAddress => RabbitMqTestHarness.BusAddress;

        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            return RabbitMqTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return RabbitMqTestHarness.Stop();
        }

        protected virtual void ConfigureRabbitMqHost(IRabbitMqHostConfigurator configurator)
        {
        }

        protected virtual void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
        }

        protected virtual void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
        }

        protected virtual void OnCleanupVirtualHost(IModel model)
        {
        }

        protected IRabbitMqHost Host => RabbitMqTestHarness.Host;

        protected IMessageNameFormatter NameFormatter => RabbitMqTestHarness.NameFormatter;
    }
}