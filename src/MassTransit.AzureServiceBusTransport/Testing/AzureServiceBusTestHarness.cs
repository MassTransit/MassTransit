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
namespace MassTransit.AzureServiceBusTransport.Testing
{
    using System;
    using Logging;
    using MassTransit.Testing;
    using Microsoft.ServiceBus;


    public class AzureServiceBusTestHarness :
        BusTestHarness
    {
        static readonly ILog _log = Logger.Get<AzureServiceBusTestHarness>();
        readonly Uri _serviceUri;

        Uri _inputQueueAddress;

        public AzureServiceBusTestHarness(Uri serviceUri, string sharedAccessKeyName, string sharedAccessKeyValue, string inputQueueName = null)
        {
            if (serviceUri == null)
                throw new ArgumentNullException(nameof(serviceUri));

            _serviceUri = serviceUri;
            SharedAccessKeyName = sharedAccessKeyName;
            SharedAccessKeyValue = sharedAccessKeyValue;

            TokenTimeToLive = TimeSpan.FromDays(1);
            TokenScope = TokenScope.Namespace;

            InputQueueName = inputQueueName ?? "input_queue";

            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Https;
        }

        public string SharedAccessKeyName { get; }
        public string SharedAccessKeyValue { get; }
        public TimeSpan TokenTimeToLive { get; set; }
        public TokenScope TokenScope { get; set; }
        public string InputQueueName { get; }
        public IServiceBusHost Host { get; private set; }

        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IServiceBusBusFactoryConfigurator> OnConfigureBus;
        public event Action<IServiceBusBusFactoryConfigurator, IServiceBusHost> OnConfigureBusHost;
        public event Action<IServiceBusReceiveEndpointConfigurator> OnConfigureInputQueueEndpoint;

        protected virtual void ConfigureBus(IServiceBusBusFactoryConfigurator configurator)
        {
            OnConfigureBus?.Invoke(configurator);
        }

        protected virtual void ConfigureBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            OnConfigureBusHost?.Invoke(configurator, host);
        }

        protected virtual void ConfigureInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            OnConfigureInputQueueEndpoint?.Invoke(configurator);
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                ConfigureBus(x);

                Host = x.Host(_serviceUri, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = SharedAccessKeyName;
                        s.SharedAccessKey = SharedAccessKeyValue;
                        s.TokenTimeToLive = TokenTimeToLive;
                        s.TokenScope = TokenScope;
                    });
                });

                x.UseServiceBusMessageScheduler();

                ConfigureBusHost(x, Host);

                x.ReceiveEndpoint(Host, InputQueueName, e =>
                {
                    ConfigureInputQueueEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }
    }
}