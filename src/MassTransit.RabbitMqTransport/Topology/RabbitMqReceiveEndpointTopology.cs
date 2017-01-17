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
namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using Builders;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using RabbitMqTransport.Specifications;
    using Transport;
    using Transports;


    public class RabbitMqReceiveEndpointTopology :
        IRabbitMqReceiveEndpointTopology
    {
        readonly IRabbitMqHost _host;
        readonly IRabbitMqPublishTopology _publish;
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly IPublishPipe _publishPipe;
        readonly IRabbitMqSendTopology _send;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly IMessageSerializer _serializer;
        readonly TopologyLayout _topologyLayout;

        public RabbitMqReceiveEndpointTopology(IRabbitMqEndpointConfiguration configuration, Uri inputAddress, IMessageSerializer serializer,
            ISendTransportProvider sendTransportProvider, IRabbitMqHost host, TopologyLayout topologyLayout)
        {
            InputAddress = inputAddress;
            _serializer = serializer;
            _sendTransportProvider = sendTransportProvider;
            _host = host;
            _topologyLayout = topologyLayout;

            _send = configuration.SendTopology;
            _publish = configuration.PublishTopology;

            _sendPipe = configuration.CreateSendPipe();
            _publishPipe = configuration.CreatePublishPipe();

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        public TopologyLayout TopologyLayout => _topologyLayout;

        public Uri InputAddress { get; }

        public ISendTopology Send => _send;
        public IPublishTopology Publish => _publish;

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;

        public ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = new RabbitMqSendEndpointProvider(_serializer, InputAddress, _sendTransportProvider, _sendPipe);

            return new SendEndpointCache(provider);
        }

        IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return new RabbitMqPublishEndpointProvider(_host, _serializer, InputAddress, _publishPipe, _publish);
        }
    }
}