// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class ActiveMqTestFixture :
        BusTestFixture
    {
        public ActiveMqTestFixture(string inputQueueName = null)
            : this(new ActiveMqTestHarness(inputQueueName))
        {
        }

        public ActiveMqTestFixture(ActiveMqTestHarness harness)
            : base(harness)
        {
            ActiveMqTestHarness = harness;

            ActiveMqTestHarness.OnConfigureActiveMqHost += ConfigureActiveMqHost;
            ActiveMqTestHarness.OnConfigureActiveMqBus += ConfigureActiveMqBus;
            ActiveMqTestHarness.OnConfigureActiveMqBusHost += ConfigureActiveMqBusHost;
            ActiveMqTestHarness.OnConfigureActiveMqReceiveEndpoint += ConfigureActiveMqReceiveEndpoint;
        }

        protected ActiveMqTestHarness ActiveMqTestHarness { get; }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => ActiveMqTestHarness.InputQueueSendEndpoint;

        protected Uri InputQueueAddress => ActiveMqTestHarness.InputQueueAddress;

        protected Uri HostAddress => ActiveMqTestHarness.HostAddress;

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => ActiveMqTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => ActiveMqTestHarness.Sent;

        protected Uri BusAddress => ActiveMqTestHarness.BusAddress;

        protected IActiveMqHost Host => ActiveMqTestHarness.Host;

        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            return ActiveMqTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return ActiveMqTestHarness.Stop();
        }

        protected virtual void ConfigureActiveMqHost(IActiveMqHostConfigurator configurator)
        {
        }

        protected virtual void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureActiveMqBusHost(IActiveMqBusFactoryConfigurator configurator, IActiveMqHost host)
        {
        }

        protected virtual void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
        }
    }
}