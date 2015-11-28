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
    using Builders;
    using BusConfigurators;
    using MassTransit.Pipeline;
    using Transports;


    public class ServiceBusBusBuilder :
        BusBuilder
    {
        readonly ServiceBusReceiveEndpointConfigurator _busEndpointConfigurator;
        readonly ServiceBusHost[] _hosts;

        public ServiceBusBusBuilder(ServiceBusHost[] hosts, IConsumePipeSpecification consumePipeSpecification, ISendPipeFactory sendPipeFactory,
            ReceiveEndpointSettings settings)
            : base(consumePipeSpecification, sendPipeFactory, hosts)
        {
            if (hosts == null)
                throw new ArgumentNullException(nameof(hosts));

            _hosts = hosts;

            _busEndpointConfigurator = new ServiceBusReceiveEndpointConfigurator(_hosts[0], settings, ConsumePipe);
        }

        protected override Uri GetInputAddress()
        {
            return _busEndpointConfigurator.InputAddress;
        }

        protected override IConsumePipe GetConsumePipe()
        {
            return CreateConsumePipe();
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new ServiceBusSendTransportProvider(_hosts);
        }

        public override ISendEndpointProvider CreateSendEndpointProvider(params ISendPipeSpecification[] specifications)
        {
            var sendPipe = CreateSendPipe(specifications);

            var provider = new ServiceBusSendEndpointProvider(MessageSerializer, InputAddress, SendTransportProvider, sendPipe);

            return new SendEndpointCache(provider);
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            var sendPipe = CreateSendPipe();

            var sendEndpointProvider = new PublishSendEndpointProvider(MessageSerializer, InputAddress, _hosts, sendPipe);

            var endpointCache = new SendEndpointCache(sendEndpointProvider);

            return new ServiceBusPublishEndpointProvider(_hosts[0], endpointCache);
        }

        protected override void PreBuild()
        {
            base.PreBuild();

            _busEndpointConfigurator.Apply(this);
        }
    }
}