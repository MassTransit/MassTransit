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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using BusConfigurators;
    using Configurators;
    using PipeConfigurators;
    using Util;


    public class ServiceBusBusFactoryConfigurator :
        IServiceBusBusFactoryConfigurator,
        IQueueConfigurator,
        IBusFactory
    {
        readonly ConsumePipeSpecificationList _consumePipeSpecification;
        readonly IList<ServiceBusHost> _hosts;
        readonly ReceiveEndpointSettings _settings;
        readonly IList<IBusFactorySpecification> _transportSpecifications;

        public ServiceBusBusFactoryConfigurator()
        {
            _hosts = new List<ServiceBusHost>();
            _transportSpecifications = new List<IBusFactorySpecification>();
            _consumePipeSpecification = new ConsumePipeSpecificationList();

            string queueName = HostMetadataCache.Host.GetTemporaryQueueName();
            _settings = new ReceiveEndpointSettings(queueName)
            {
                QueueDescription =
                {
                    EnableExpress = true,
                    AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
                }
            };
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _transportSpecifications.SelectMany(x => x.Validate());
        }

        public IBusControl CreateBus()
        {
            var builder = new ServiceBusBusBuilder(_hosts, _consumePipeSpecification, _settings);

            foreach (IBusFactorySpecification configurator in _transportSpecifications)
                configurator.Apply(builder);

            return builder.Build();
        }

        public bool EnableExpress
        {
            set { _settings.QueueDescription.EnableExpress = value; }
        }

        public TimeSpan LockDuration
        {
            set { _settings.QueueDescription.LockDuration = value; }
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set { _settings.QueueDescription.EnableDeadLetteringOnMessageExpiration = value; }
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set { _settings.QueueDescription.DefaultMessageTimeToLive = value; }
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _settings.QueueDescription.RequiresDuplicateDetection = true;
            _settings.QueueDescription.DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }

        public TimeSpan AutoDeleteOnIdle
        {
            set { _settings.QueueDescription.AutoDeleteOnIdle = value; }
        }

        public int PrefetchCount
        {
            set { _settings.PrefetchCount = value; }
        }

        public int MaxConcurrentCalls
        {
            set
            {
                _settings.MaxConcurrentCalls = value;
                if (_settings.MaxConcurrentCalls > _settings.PrefetchCount)
                    _settings.PrefetchCount = _settings.MaxConcurrentCalls;
            }
        }

        public IServiceBusHost Host(ServiceBusHostSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var host = new ServiceBusHost(settings);
            _hosts.Add(host);

            return host;
        }

        void IBusFactoryConfigurator.AddBusFactorySpecification(IBusFactorySpecification configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            _transportSpecifications.Add(configurator);
        }

        void IPipeConfigurator<ConsumeContext>.AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _consumePipeSpecification.Add(specification);
        }

        void IConsumePipeConfigurator.AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification)
        {
            _consumePipeSpecification.Add(specification);
        }
    }
}