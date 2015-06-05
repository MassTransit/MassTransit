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
    using Configuration;
    using Microsoft.ServiceBus;
    using NUnit.Framework;


    [TestFixture]
    public class TwoScopeAzureServiceBusTestFixture :
        AzureServiceBusTestFixture
    {
        public TwoScopeAzureServiceBusTestFixture()
        {
            _secondServiceUri = ServiceBusEnvironment.CreateServiceUri("sb", "masstransit-build", "MassTransit.Tests.SecondService");
        }

        Uri _secondInputQueueAddress;
        readonly Uri _secondServiceUri;
        IBusControl _secondBus;
        BusHandle _secondBusHandle;
        ISendEndpoint _secondBusSendEndpoint;
        ISendEndpoint _secondInputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint SecondInputQueueSendEndpoint
        {
            get { return _secondInputQueueSendEndpoint; }
        }

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint SecondBusSendEndpoint
        {
            get { return _secondBusSendEndpoint; }
        }

        protected Uri SecondBusAddress
        {
            get { return _secondBus.Address; }
        }

        protected Uri SecondInputQueueAddress
        {
            get { return _secondInputQueueAddress; }
            set
            {
                if (Bus != null)
                    throw new InvalidOperationException("The LocalBus has already been created, too late to change the URI");

                _secondInputQueueAddress = value;
            }
        }

        protected virtual IBus SecondBus
        {
            get { return _secondBus; }
        }

        [TestFixtureSetUp]
        public void SetupSecondAzureServiceBusTestFixture()
        {
            _secondBus = CreateSecondBus();

            _secondBusHandle = _secondBus.Start();
            try
            {
                _secondBusSendEndpoint = Await(() => _secondBus.GetSendEndpoint(_secondBus.Address));

                _secondInputQueueSendEndpoint = Await(() => _secondBus.GetSendEndpoint(_secondInputQueueAddress));
            }
            catch (Exception ex)
            {
                Console.WriteLine("The bus creation failed: {0}", ex);

                Await(() => _secondBusHandle.Stop());

                throw;
            }
        }

        [TestFixtureTearDown]
        public void TearDownInMemoryTestFixture()
        {
            if (_secondBusHandle != null)
                Await(() => _secondBusHandle.Stop());

            _secondBus = null;
        }

        protected virtual void ConfigureSecondBus(IServiceBusBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureSecondBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
        }

        protected virtual void ConfigureSecondInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }

        IBusControl CreateSecondBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                ConfigureSecondBus(x);

                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                IServiceBusHost host = x.Host(_secondServiceUri, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = settings.KeyName;
                        s.SharedAccessKey = settings.SharedAccessKey;
                        s.TokenTimeToLive = settings.TokenTimeToLive;
                        s.TokenScope = settings.TokenScope;
                    });
                });

                ConfigureSecondBusHost(x, host);

                x.ReceiveEndpoint(host, "input_queue", e =>
                {
                    _secondInputQueueAddress = e.QueuePath;

                    ConfigureSecondInputQueueEndpoint(e);
                });
            });
        }
    }
}