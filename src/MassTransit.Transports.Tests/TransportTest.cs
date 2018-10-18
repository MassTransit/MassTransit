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
        readonly Type _harnessType;

        public TransportTest(Type harnessType)
        {
            _harnessType = harnessType;
        }

        protected BusTestHarness Harness { get; private set; }

        [OneTimeSetUp]
        public void CreateHarness()
        {
            if (_harnessType == typeof(InMemoryTestHarness))
                Harness = new InMemoryTestHarness();
            else if (_harnessType == typeof(RabbitMqTestHarness))
                Harness = new RabbitMqTestHarness();
            else if (_harnessType == typeof(ActiveMqTestHarness))
                Harness = new ActiveMqTestHarness();
            else if (_harnessType == typeof(AzureServiceBusTestHarness))
            {
                var serviceUri = new Uri("sb://masstransit-build.servicebus.windows.net");

                Harness = new AzureServiceBusTestHarness(serviceUri, "MassTransitBuild", "u07PCkrzic95bk6UtpDXl8YdHmV40WtJihoGz2CnMMQ=");
            }
            else if (_harnessType == typeof(AmazonSqsTestHarness))
            {
                Harness = new AmazonSqsTestHarness("us-east-2", "AKIAJGLM67TI5XDCBVJA", "MxbNRh1ufprfT0axXmoEF52QTDAlJ0lRVyPdlyOZ");
            }
            else
            {
                throw new ArgumentException($"Unknown test harness type: {TypeCache.GetShortName(_harnessType)}");
            }
        }
    }
}