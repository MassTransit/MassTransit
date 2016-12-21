// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using MassTransit.Testing;
    using Microsoft.ServiceBus;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    [TestFixture]
    public abstract class AzureServiceBusTestFixture :
        BusTestFixture
    {
        protected AzureServiceBusTestHarness AzureServiceBusTestHarness { get; }

        protected override BusTestHarness BusTestHarness => AzureServiceBusTestHarness;

        static readonly ILog _log = Logger.Get<AzureServiceBusTestFixture>();

        public AzureServiceBusTestFixture(string inputQueueName = null, Uri serviceUri = null, ServiceBusTokenProviderSettings settings = null)
        {
            serviceUri = serviceUri ?? ServiceBusEnvironment.CreateServiceUri("sb", "masstransit-build", "MassTransit.AzureServiceBusTransport.Tests");

            settings = settings ?? new TestAzureServiceBusAccountSettings();

            AzureServiceBusTestHarness = new AzureServiceBusTestHarness(serviceUri, settings.KeyName, settings.SharedAccessKey, inputQueueName);

            AzureServiceBusTestHarness.OnConnectObservers += ConnectObservers;
            AzureServiceBusTestHarness.OnConfigureBus += ConfigureBus;
            AzureServiceBusTestHarness.OnConfigureBusHost += ConfigureBusHost;
            AzureServiceBusTestHarness.OnConfigureInputQueueEndpoint += ConfigureInputQueueEndpoint;
        }

        protected string InputQueueName => AzureServiceBusTestHarness.InputQueueName;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => AzureServiceBusTestHarness.InputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => AzureServiceBusTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => AzureServiceBusTestHarness.Sent;

        protected Uri BusAddress => AzureServiceBusTestHarness.BusAddress;

        protected Uri InputQueueAddress => AzureServiceBusTestHarness.InputQueueAddress;

        [OneTimeSetUp]
        public Task SetupAzureServiceBusTestFixture()
        {
            return AzureServiceBusTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return AzureServiceBusTestHarness.Stop();
        }

        protected virtual void ConfigureBus(IServiceBusBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
        }

        protected virtual void ConfigureInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }

        protected IServiceBusHost Host => AzureServiceBusTestHarness.Host;
    }
}