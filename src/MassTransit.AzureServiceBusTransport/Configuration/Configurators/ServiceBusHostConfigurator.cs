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
namespace MassTransit.AzureServiceBusTransport.Configuration.Configurators
{
    using System;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.ServiceBus.Messaging.Amqp;


    public class ServiceBusHostConfigurator :
        IServiceBusHostConfigurator
    {
        readonly HostSettings _settings;

        public ServiceBusHostConfigurator(Uri serviceAddress)
        {
            _settings = new HostSettings(serviceAddress);
        }

        public ServiceBusHostSettings Settings => _settings;

        public TokenProvider TokenProvider
        {
            set { _settings.TokenProvider = value; }
        }

        public TimeSpan OperationTimeout
        {
            set { _settings.OperationTimeout = value; }
        }

        public TransportType TransportType
        {
            set { _settings.TransportType = value; }
        }

        public TimeSpan RetryMinBackoff
        {
            set { _settings.RetryMinBackoff = value; }
        }

        public TimeSpan RetryMaxBackoff
        {
            set { _settings.RetryMaxBackoff = value; }
        }

        public int RetryLimit
        {
            set { _settings.RetryLimit = value; }
        }

        public TimeSpan BatchFlushInterval
        {
            set
            {
                _settings.AmqpTransportSettings.BatchFlushInterval = value;
                _settings.NetMessagingTransportSettings.BatchFlushInterval = value;
            }
        }


        class HostSettings :
            ServiceBusHostSettings
        {
            public HostSettings(Uri serviceUri)
            {
                ServiceUri = serviceUri;
                OperationTimeout = TimeSpan.FromSeconds(60);

                RetryMinBackoff = TimeSpan.FromMilliseconds(100);
                RetryMaxBackoff = TimeSpan.FromSeconds(30);
                RetryLimit = 10;

                TransportType = TransportType.Amqp;
                AmqpTransportSettings = new AmqpTransportSettings();
                NetMessagingTransportSettings = new NetMessagingTransportSettings();
            }

            public Uri ServiceUri { get; private set; }
            public TokenProvider TokenProvider { get; set; }
            public TimeSpan OperationTimeout { get; set; }
            public TimeSpan RetryMinBackoff { get; set; }
            public TimeSpan RetryMaxBackoff { get; set; }
            public int RetryLimit { get; set; }
            public TransportType TransportType { get; set; }
            public AmqpTransportSettings AmqpTransportSettings { get; set; }
            public NetMessagingTransportSettings NetMessagingTransportSettings { get; set; }
        }
    }
}
