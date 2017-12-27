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
        readonly IRabbitMqConsumeTopology _consume;
        readonly IRabbitMqSendTopology _send;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly Lazy<ISendTransportProvider> _sendTransportProvider;
        readonly IRabbitMqEndpointConfiguration _configuration;
        readonly IMessageSerializer _serializer;
        readonly BrokerTopology _brokerTopology;
        readonly BusHostCollection<RabbitMqHost> _hosts;

        public RabbitMqReceiveEndpointTopology(IRabbitMqEndpointConfiguration configuration, Uri inputAddress, IMessageSerializer serializer,
            IRabbitMqHost host, BusHostCollection<RabbitMqHost> hosts, BrokerTopology brokerTopology)
        {
            InputAddress = inputAddress;
            _configuration = configuration;
            _serializer = serializer;
            _host = host;
            _brokerTopology = brokerTopology;

            _hosts = hosts;

            _consume = configuration.Topology.Consume;

            _send = configuration.Topology.Send;
            _publish = configuration.Topology.Publish;

            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);
            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        public Uri InputAddress { get; }
        public BrokerTopology BrokerTopology => _brokerTopology;

        public IRabbitMqConsumeTopology ConsumeTopology => _consume;
        public IRabbitMqSendTopology SendTopology => _send;
        public IRabbitMqPublishTopology PublishTopology => _publish;

        ISendTopology IReceiveEndpointTopology.Send => _send;
        IPublishTopology IReceiveEndpointTopology.Publish => _publish;

        ISendEndpointProvider IReceiveEndpointTopology.SendEndpointProvider => _sendEndpointProvider.Value;
        IPublishEndpointProvider IReceiveEndpointTopology.PublishEndpointProvider => _publishEndpointProvider.Value;
        ISendTransportProvider IReceiveEndpointTopology.SendTransportProvider => _sendTransportProvider.Value;

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            var sendPipe = _configuration.Send.CreatePipe();

            var provider = new RabbitMqSendEndpointProvider(_serializer, InputAddress, _sendTransportProvider.Value, sendPipe);

            return new SendEndpointCache(provider);
        }

        IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            IPublishPipe publishPipe = _configuration.Publish.CreatePipe();

            return new RabbitMqPublishEndpointProvider(_host, _serializer, InputAddress, publishPipe);
        }

        ISendTransportProvider CreateSendTransportProvider()
        {
            return new RabbitMqSendTransportProvider(_hosts);
        }
    }
}