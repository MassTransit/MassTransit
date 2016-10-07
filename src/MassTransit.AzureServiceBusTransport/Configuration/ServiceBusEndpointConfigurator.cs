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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using EndpointConfigurators;
    using MassTransit.Pipeline;


    public abstract class ServiceBusEndpointConfigurator :
        ReceiveEndpointConfigurator,
        IServiceBusEndpointConfigurator
    {
        readonly IServiceBusHost _host;
        readonly BaseClientSettings _settings;

        protected ServiceBusEndpointConfigurator(IServiceBusHost host, BaseClientSettings settings, IConsumePipe consumePipe = null)
            : base(consumePipe)
        {
            _host = host;
            _settings = settings;
        }

        public IServiceBusHost Host => _host;

        public int MaxConcurrentCalls
        {
            set
            {
                _settings.MaxConcurrentCalls = value;
                if (_settings.MaxConcurrentCalls > _settings.PrefetchCount)
                    _settings.PrefetchCount = _settings.MaxConcurrentCalls;
            }
        }

        public int PrefetchCount
        {
            set { _settings.PrefetchCount = value; }
        }

        public TimeSpan AutoDeleteOnIdle
        {
            set { _settings.AutoDeleteOnIdle = value; }
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set { _settings.DefaultMessageTimeToLive = value; }
        }

        public bool EnableBatchedOperations
        {
            set { _settings.EnableBatchedOperations = value; }
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set { _settings.EnableDeadLetteringOnMessageExpiration = value; }
        }

        public string ForwardDeadLetteredMessagesTo
        {
            set { _settings.ForwardDeadLetteredMessagesTo = value; }
        }

        public TimeSpan LockDuration
        {
            set { _settings.LockDuration = value; }
        }

        public int MaxDeliveryCount
        {
            set { _settings.MaxDeliveryCount = value; }
        }

        public bool RequiresSession
        {
            set { _settings.RequiresSession = value; }
        }

        public string UserMetadata
        {
            set { _settings.UserMetadata = value; }
        }
    }
}