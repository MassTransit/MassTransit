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
namespace MassTransit.AzureServiceBusTransport.Tests
{
    using System;
    using Configuration;
    using Microsoft.ServiceBus;
    using NUnit.Framework;
    using TestFramework;
    using Testing;
    using Testing.TestDecorators;


    [TestFixture]
    public class AzureServiceBusTestFixture :
        BusTestFixture
    {
        IBusControl _bus;
        Uri _inputQueueAddress;
        ISendEndpoint _inputQueueSendEndpoint;
        ISendEndpoint _busSendEndpoint;
        readonly TestSendObserver _sendObserver;
        readonly Uri _serviceUri;
        BusHandle _busHandle;

        public AzureServiceBusTestFixture()
        {
            TestTimeout = TimeSpan.FromMinutes(1);

            _serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", "masstransit-build", "MassTransit.AzureServiceBusTransport.Tests");

            _sendObserver = new TestSendObserver(TestTimeout);
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint
        {
            get { return _inputQueueSendEndpoint; }
        }

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint BusSendEndpoint
        {
            get { return _busSendEndpoint; }
        }

        protected ISentMessageList Sent
        {
            get { return _sendObserver.Messages; }
        }

        protected Uri BusAddress
        {
            get { return _bus.Address; }
        }

        protected Uri InputQueueAddress
        {
            get { return _inputQueueAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _inputQueueAddress = value;
            }
        }

        protected override IBus Bus
        {
            get { return _bus; }
        }

        [TestFixtureSetUp]
        public void SetupInMemoryTestFixture()
        {
            _bus = CreateBus();

            var startTask = _bus.Start(TestCancellationToken);

            startTask.Wait(TestCancellationToken);

            _busHandle = startTask.Result;
            try
            {
                _busSendEndpoint = _bus.GetSendEndpoint(_bus.Address).Result;
                _busSendEndpoint.Connect(_sendObserver);

                _inputQueueSendEndpoint = _bus.GetSendEndpoint(_inputQueueAddress).Result;
                _inputQueueSendEndpoint.Connect(_sendObserver);
            }
            catch (Exception)
            {
                _busHandle.Stop();

                throw;
            }
        }

        [TestFixtureTearDown]
        public void TearDownInMemoryTestFixture()
        {
            try
            {
                if (_busHandle != null)
                    _busHandle.Stop().Wait(TestTimeout);
            }
            catch (AggregateException ex)
            {
                throw;
            }

            _bus = null;
        }

        protected virtual void ConfigureBus(IServiceBusBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                AzureServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                ServiceBusHostSettings host = x.Host(_serviceUri, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = settings.KeyName;
                        s.SharedAccessKey = settings.SharedAccessKey;
                        s.TokenTimeToLive = settings.TokenTimeToLive;
                        s.TokenScope = settings.TokenScope;
                    });
                });

                ConfigureBus(x);

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    _inputQueueAddress = e.QueuePath;

                    ConfigureInputQueueEndpoint(e);
                });
            });
        }
    }
}