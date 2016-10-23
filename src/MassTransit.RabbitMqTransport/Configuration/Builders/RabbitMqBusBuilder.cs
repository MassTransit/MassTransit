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
namespace MassTransit.RabbitMqTransport.Builders
{
    using System;
    using BusConfigurators;
    using Configurators;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Topology;
    using Transport;
    using Transports;


    public class RabbitMqBusBuilder :
        BusBuilder
    {
        readonly TimeSpan _autoDeleteCacheTimeout;
        readonly RabbitMqReceiveEndpointConfigurator _busEndpointConfigurator;
        readonly BusHostCollection<RabbitMqHost> _hosts;
        readonly ModelSettings _modelSettings;
        readonly TimeSpan _sendEndpointCacheTimeout;

        public RabbitMqBusBuilder(BusHostCollection<RabbitMqHost> hosts, IConsumePipeFactory consumePipeFactory, ISendPipeFactory sendPipeFactory,
            IPublishPipeFactory publishPipeFactory, RabbitMqReceiveSettings busSettings, ModelSettings modelSettings)
            : base(consumePipeFactory, sendPipeFactory, publishPipeFactory, hosts)
        {
            _hosts = hosts;
            _modelSettings = modelSettings;

            _autoDeleteCacheTimeout = TimeSpan.FromMinutes(1);
            _sendEndpointCacheTimeout = TimeSpan.FromDays(1);

            _busEndpointConfigurator = new RabbitMqReceiveEndpointConfigurator(_hosts[0], busSettings, ConsumePipe);

            ReceiveEndpointFactory = new RabbitMqReceiveEndpointFactory(this);
        }

        public BusHostCollection<RabbitMqHost> Hosts => _hosts;

        protected override void PreBuild()
        {
            _busEndpointConfigurator.Apply(this);
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
            return new RabbitMqSendTransportProvider(_hosts, _modelSettings);
        }

        public override ISendEndpointProvider CreateSendEndpointProvider(Uri sourceAddress, params ISendPipeSpecification[] specifications)
        {
            ISendPipe pipe = CreateSendPipe(specifications);

            var provider = new RabbitMqSendEndpointProvider(MessageSerializer, sourceAddress, SendTransportProvider, pipe);

            return new SendEndpointCache(provider, CacheDurationProvider);
        }

        TimeSpan CacheDurationProvider(Uri address)
        {
            if (address.GetReceiveSettings().AutoDelete)
                return _autoDeleteCacheTimeout;

            return _sendEndpointCacheTimeout;
        }

        public override IPublishEndpointProvider CreatePublishEndpointProvider(Uri sourceAddress, params IPublishPipeSpecification[] specifications)
        {
            IPublishPipe pipe = CreatePublishPipe(specifications);

            return new RabbitMqPublishEndpointProvider(_hosts[0], MessageSerializer, InputAddress, pipe, _modelSettings);
        }
    }
}