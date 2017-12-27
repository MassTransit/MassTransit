// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Configuration.Builders
{
    using System;
    using Configurators;
    using MassTransit.Builders;
    using Settings;
    using Specifications;
    using Transport;
    using Transports;


    public class ServiceBusBusBuilder :
        BusBuilder
    {
        readonly ServiceBusReceiveEndpointSpecification _busEndpointSpecification;

        public ServiceBusBusBuilder(BusHostCollection<ServiceBusHost> hosts, ReceiveEndpointSettings settings,
            IServiceBusEndpointConfiguration configuration, ISendTransportProvider sendTransportProvider)
            : base(hosts, configuration)
        {
            if (hosts == null)
                throw new ArgumentNullException(nameof(hosts));

            var endpointTopologySpecification = configuration.CreateNewConfiguration(ConsumePipe);

            _busEndpointSpecification = new ServiceBusReceiveEndpointSpecification(hosts[0], settings, endpointTopologySpecification, sendTransportProvider);

            foreach (var host in hosts.Hosts)
            {
                host.ReceiveEndpointFactory = new ServiceBusReceiveEndpointFactory(this, host, configuration, sendTransportProvider);
                host.SubscriptionEndpointFactory = new ServiceBusSubscriptionEndpointFactory(this, host, configuration, sendTransportProvider);
            }
        }

        public override IPublishEndpointProvider PublishEndpointProvider => _busEndpointSpecification.PublishEndpointProvider;
        public override ISendEndpointProvider SendEndpointProvider => _busEndpointSpecification.SendEndpointProvider;

        protected override Uri GetInputAddress()
        {
            return _busEndpointSpecification.InputAddress;
        }

        protected override void PreBuild()
        {
            _busEndpointSpecification.Apply(this);
        }
    }
}