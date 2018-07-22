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
namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class AmazonSqsTestFixture :
        BusTestFixture
    {
        public AmazonSqsTestFixture(string inputQueueName = null)
            : this(new AmazonSqsTestHarness(inputQueueName))
        {
        }

        public AmazonSqsTestFixture(AmazonSqsTestHarness harness)
            : base(harness)
        {
            AmazonSqsTestHarness = harness;

            AmazonSqsTestHarness.OnConfigureAmazonSqsHost += ConfigureAmazonSqsHost;
            AmazonSqsTestHarness.OnConfigureAmazonSqsBus += ConfigureAmazonSqsBus;
            AmazonSqsTestHarness.OnConfigureAmazonSqsBusHost += ConfigureAmazonSqsBusHost;
            AmazonSqsTestHarness.OnConfigureAmazonSqsReceiveEndoint += ConfigureAmazonSqsReceiveEndpoint;
        }

        protected AmazonSqsTestHarness AmazonSqsTestHarness { get; }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => AmazonSqsTestHarness.InputQueueSendEndpoint;

        protected Uri InputQueueAddress => AmazonSqsTestHarness.InputQueueAddress;

        protected Uri HostAddress => AmazonSqsTestHarness.HostAddress;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => AmazonSqsTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => AmazonSqsTestHarness.Sent;

        protected Uri BusAddress => AmazonSqsTestHarness.BusAddress;

        protected IAmazonSqsHost Host => AmazonSqsTestHarness.Host;

        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            return AmazonSqsTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return AmazonSqsTestHarness.Stop();
        }

        protected virtual void ConfigureAmazonSqsHost(IAmazonSqsHostConfigurator configurator)
        {
        }

        protected virtual void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureAmazonSqsBusHost(IAmazonSqsBusFactoryConfigurator configurator, IAmazonSqsHost host)
        {
        }

        protected virtual void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
        }
    }
}