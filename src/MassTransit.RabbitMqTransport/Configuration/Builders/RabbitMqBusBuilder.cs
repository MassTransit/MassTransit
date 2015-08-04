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
namespace MassTransit.RabbitMqTransport.Configuration.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BusConfigurators;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Topology;
    using Transports;


    public class RabbitMqBusBuilder :
        BusBuilder,
        IBusBuilder
    {
        readonly IConsumePipe _busConsumePipe;
        readonly RabbitMqReceiveEndpointConfigurator _busEndpointConfigurator;
        readonly RabbitMqHost[] _hosts;

        public RabbitMqBusBuilder(IEnumerable<RabbitMqHost> hosts, IConsumePipeSpecification consumePipeSpecification, RabbitMqReceiveSettings busSettings)
            : base(consumePipeSpecification)
        {
            _hosts = hosts.ToArray();

            _busConsumePipe = CreateConsumePipe();

            _busEndpointConfigurator = new RabbitMqReceiveEndpointConfigurator(_hosts[0], busSettings, _busConsumePipe);
        }

        public IBusControl Build()
        {
            _busEndpointConfigurator.Apply(this);

            return new MassTransitBus(_busEndpointConfigurator.InputAddress, _busConsumePipe, SendEndpointProvider, PublishEndpoint, ReceiveEndpoints, _hosts);
        }

        protected override Uri GetInputAddress()
        {
            return _busEndpointConfigurator.InputAddress;
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new RabbitMqSendTransportProvider(_hosts);
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            var sendEndpointProvider = new RabbitMqSendEndpointProvider(MessageSerializer, InputAddress, SendTransportProvider);

            return new SendEndpointCache(sendEndpointProvider);
        }

        protected override IPublishEndpointProvider CreatePublishSendEndpointProvider()
        {
            return new RabbitMqPublishEndpointProvider(_hosts[0], MessageSerializer, InputAddress);
        }
    }
}