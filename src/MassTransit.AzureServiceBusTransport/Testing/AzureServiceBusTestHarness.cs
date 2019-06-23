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
    using MassTransit.Testing;
    using Microsoft.ServiceBus;


    public class AzureServiceBusTestHarness :
        BusTestHarness
    {
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

            ConfigureMessageScheduler = true;
        }

        public string SharedAccessKeyName { get; }
        public string SharedAccessKeyValue { get; }
        public TimeSpan TokenTimeToLive { get; set; }
        public TokenScope TokenScope { get; set; }
        public override string InputQueueName { get; }
        public IServiceBusHost Host { get; private set; }
        public bool ConfigureMessageScheduler { get; set; }

        public override Uri InputQueueAddress => _inputQueueAddress;

        public event Action<IServiceBusBusFactoryConfigurator> OnConfigureServiceBusBus;
        public event Action<IServiceBusBusFactoryConfigurator, IServiceBusHost> OnConfigureServiceBusBusHost;
        public event Action<IServiceBusReceiveEndpointConfigurator> OnConfigureServiceBusReceiveEndpoint;

        protected virtual void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            OnConfigureServiceBusBus?.Invoke(configurator);
        }

        protected virtual void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            OnConfigureServiceBusBusHost?.Invoke(configurator, host);
        }

        protected virtual void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            OnConfigureServiceBusReceiveEndpoint?.Invoke(configurator);
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
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

                ConfigureBus(x);

                ConfigureServiceBusBus(x);

                if (ConfigureMessageScheduler)
                    x.UseServiceBusMessageScheduler();

                ConfigureServiceBusBusHost(x, Host);

                x.ReceiveEndpoint(Host, InputQueueName, e =>
                {
                    ConfigureReceiveEndpoint(e);

                    ConfigureServiceBusReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }
    }
}
