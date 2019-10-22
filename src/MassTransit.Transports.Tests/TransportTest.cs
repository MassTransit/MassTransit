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
namespace MassTransit.Transports.Tests
{
    using System;
    using ActiveMqTransport.Testing;
    using AmazonSqsTransport.Testing;
    using Azure.ServiceBus.Core.Testing;
    using GreenPipes.Internals.Extensions;
    using NUnit.Framework;
    using RabbitMqTransport.Testing;
    using Testing;


    [TestFixture(typeof(InMemoryTestHarness))]
    [TestFixture(typeof(RabbitMqTestHarness))]
    [TestFixture(typeof(ActiveMqTestHarness))]
    [TestFixture(typeof(AzureServiceBusTestHarness))]
    [TestFixture(typeof(AmazonSqsTestHarness))]
    public class TransportTest
    {
        protected Type HarnessType { get; }

        public TransportTest(Type harnessType)
        {
            HarnessType = harnessType;

            Subscribe = true;
        }

        protected BusTestHarness Harness { get; private set; }

        /// <summary>
        /// Set to False if the receive endpoint should not subscribe to message topics/exchanges
        /// </summary>
        protected bool Subscribe { private get; set; }

        [OneTimeSetUp]
        public void CreateHarness()
        {
            if (HarnessType == typeof(InMemoryTestHarness))
                Harness = new InMemoryTestHarness();
            else if (HarnessType == typeof(RabbitMqTestHarness))
            {
                var harness = new RabbitMqTestHarness();

                Harness = harness;

                harness.OnConfigureRabbitMqReceiveEndpoint += x => x.BindMessageExchanges = Subscribe;
            }
            else if (HarnessType == typeof(ActiveMqTestHarness))
            {
                var harness = new ActiveMqTestHarness();

                Harness = harness;

                harness.OnConfigureActiveMqReceiveEndpoint += x => x.BindMessageTopics = Subscribe;
            }
            else if (HarnessType == typeof(AzureServiceBusTestHarness))
            {
                var serviceUri = Credentials.AzureServiceUri;

                var harness = new AzureServiceBusTestHarness(serviceUri, Credentials.AzureKeyName, Credentials.AzureKeyValue);

                Harness = harness;

                harness.OnConfigureServiceBusReceiveEndpoint += x => x.SubscribeMessageTopics = Subscribe;
            }
            else if (HarnessType == typeof(AmazonSqsTestHarness))
            {
                var harness = new AmazonSqsTestHarness(Credentials.AmazonRegion, Credentials.AmazonAccessKey, Credentials.AmazonSecretKey);

                Harness = harness;

                harness.OnConfigureAmazonSqsReceiveEndpoint += x => x.SubscribeMessageTopics = Subscribe;
            }
            else
            {
                throw new ArgumentException($"Unknown test harness type: {TypeCache.GetShortName(HarnessType)}");
            }
        }
    }
}