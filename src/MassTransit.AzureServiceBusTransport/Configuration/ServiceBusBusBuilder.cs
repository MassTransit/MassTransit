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
    using MassTransit.Pipeline;
    using Transports;


    public class ServiceBusBusBuilder :
        BusBuilder,
        IBusBuilder
    {
        readonly IConsumePipe _busConsumePipe;
        readonly ServiceBusReceiveEndpointConfigurator _busEndpointConfigurator;
        readonly ServiceBusHost[] _hosts;
        readonly Uri _inputAddress;

        public ServiceBusBusBuilder(IEnumerable<ServiceBusHost> hosts, IConsumePipeSpecification consumePipeSpecification, ReceiveEndpointSettings settings)
            : base(consumePipeSpecification)
        {
            if (hosts == null)
                throw new ArgumentNullException(nameof(hosts));

            _hosts = hosts.ToArray();

            _busConsumePipe = CreateConsumePipe();

            _busEndpointConfigurator = new ServiceBusReceiveEndpointConfigurator(_hosts[0], settings, _busConsumePipe);

            _inputAddress = _busEndpointConfigurator.InputAddress;
        }

        protected override Uri GetInputAddress()
        {
            return _inputAddress;
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new ServiceBusSendTransportProvider(_hosts);
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = new ServiceBusSendEndpointProvider(MessageSerializer, _inputAddress, SendTransportProvider);

            return new SendEndpointCache(provider);
        }

        protected override IPublishEndpointProvider CreatePublishSendEndpointProvider()
        {
            var sendEndpointProvider = new PublishSendEndpointProvider(MessageSerializer, _inputAddress, _hosts);

            var endpointCache = new SendEndpointCache(sendEndpointProvider);

            return new ServiceBusPublishEndpointProvider(_hosts[0], endpointCache);
        }

        public virtual IBusControl Build()
        {
            _busEndpointConfigurator.Apply(this);

            return new MassTransitBus(_inputAddress, _busConsumePipe, SendEndpointProvider, PublishEndpoint, ReceiveEndpoints, _hosts, BusObservable);
        }
    }
}