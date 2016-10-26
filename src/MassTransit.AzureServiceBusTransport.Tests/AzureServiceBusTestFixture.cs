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
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.ServiceBus;
    using NUnit.Framework;
    using TestFramework;
    using Testing;
    using Testing.TestDecorators;


    [TestFixture]
    public class AzureServiceBusTestFixture :
        BusTestFixture
    {
        static readonly ILog _log = Logger.Get<AzureServiceBusTestFixture>();
        IBusControl _bus;
        Uri _inputQueueAddress;
        ISendEndpoint _inputQueueSendEndpoint;
        ISendEndpoint _busSendEndpoint;
        readonly TestSendObserver _sendObserver;
        readonly Uri _serviceUri;
        BusHandle _busHandle;
        readonly string _inputQueueName;
        IServiceBusHost _host;

        public AzureServiceBusTestFixture()
            : this("input_queue")
        {
        }

        public AzureServiceBusTestFixture(string inputQueueName)
        {
            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Https;

            _inputQueueName = inputQueueName;

            TestTimeout = Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(60);

            _serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", "masstransit-build", "MassTransit.AzureServiceBusTransport.Tests");

            _sendObserver = new TestSendObserver(TestTimeout);
        }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => _inputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the Bus 
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => _busSendEndpoint;

        protected ISentMessageList Sent => _sendObserver.Messages;

        protected Uri BusAddress => _bus.Address;

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

        protected override IBus Bus => _bus;

        [OneTimeSetUp]
        public async Task SetupAzureServiceBusTestFixture()
        {
            _bus = CreateBus();

            _bus.ConnectReceiveEndpointObserver(new ReceiveEndpointObserver());

            _busHandle = await _bus.StartAsync();
            try
            {
                _busSendEndpoint = await _bus.GetSendEndpoint(_bus.Address);
                _busSendEndpoint.ConnectSendObserver(_sendObserver);

                _inputQueueSendEndpoint = await _bus.GetSendEndpoint(_inputQueueAddress);
                _inputQueueSendEndpoint.ConnectSendObserver(_sendObserver);
            }
            catch (Exception)
            {
                try
                {
                    using (var tokenSource = new CancellationTokenSource(TestTimeout))
                    {
                        await _bus.StopAsync(tokenSource.Token);
                    }
                }
                finally
                {
                    _busHandle = null;
                    _bus = null;
                }

                throw;
            }
        }

        [OneTimeTearDown]
        public async Task TearDownInMemoryTestFixture()
        {
            try
            {
                using (var tokenSource = new CancellationTokenSource(TestTimeout))
                {
                    await _busHandle?.StopAsync(tokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Bus Stop Failed", ex);
            }
            finally
            {
                _busHandle = null;
                _bus = null;
            }
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

        protected IServiceBusHost Host => _host;

        IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                ConfigureBus(x);

                ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

                _host = x.Host(_serviceUri, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = settings.KeyName;
                        s.SharedAccessKey = settings.SharedAccessKey;
                        s.TokenTimeToLive = settings.TokenTimeToLive;
                        s.TokenScope = settings.TokenScope;
                    });
                });

                x.UseServiceBusMessageScheduler();

                ConfigureBusHost(x, _host);

                x.ReceiveEndpoint(_host, _inputQueueName, e =>
                {
                    _inputQueueAddress = e.InputAddress;

                    ConfigureInputQueueEndpoint(e);
                });
            });
        }


        class ReceiveEndpointObserver :
            IReceiveEndpointObserver
        {
            public Task Ready(ReceiveEndpointReady ready)
            {
                return Console.Out.WriteLineAsync($"Endpoint Ready: {ready.InputAddress}");
            }

            public Task Completed(ReceiveEndpointCompleted completed)
            {
                return Console.Out.WriteLineAsync($"Endpoint Complete: {completed.DeliveryCount}/{completed.ConcurrentDeliveryCount} - {completed.InputAddress}");
            }

            public Task Faulted(ReceiveEndpointFaulted faulted)
            {
                return Console.Out.WriteLineAsync($"Endpoint Faulted: {faulted.Exception} - {faulted.InputAddress}");
            }
        }
    }
}