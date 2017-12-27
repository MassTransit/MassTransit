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
namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using AzureServiceBusTransport.Configuration.Specifications;
    using MassTransit.Pipeline;
    using MassTransit.Topology;
    using Transport;
    using Transports;


    public class ServiceBusReceiveEndpointTopology :
        IServiceBusReceiveEndpointTopology
    {
        readonly IServiceBusHost _host;
        readonly IServiceBusPublishTopology _publish;
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly IPublishPipe _publishPipe;
        readonly IServiceBusSendTopology _send;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly IMessageSerializer _serializer;

        public ServiceBusReceiveEndpointTopology(IServiceBusEndpointConfiguration configuration, Uri inputAddress, IMessageSerializer serializer,
            IServiceBusHost host, ISendTransportProvider sendTransportProvider, BrokerTopology brokerTopology)
        {
            InputAddress = inputAddress;
            _serializer = serializer;
            _host = host;
            BrokerTopology = brokerTopology;
            _sendTransportProvider = sendTransportProvider;

            _send = configuration.Topology.Send;
            _publish = configuration.Topology.Publish;

            _sendPipe = configuration.Send.CreatePipe();
            _publishPipe = configuration.Publish.CreatePipe();

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        public BrokerTopology BrokerTopology { get; }
        public Uri InputAddress { get; }
        public ISendTopology Send => _send;
        public IPublishTopology Publish => _publish;

        ISendEndpointProvider IReceiveEndpointTopology.SendEndpointProvider => _sendEndpointProvider.Value;
        IPublishEndpointProvider IReceiveEndpointTopology.PublishEndpointProvider => _publishEndpointProvider.Value;
        ISendTransportProvider IReceiveEndpointTopology.SendTransportProvider => _sendTransportProvider;

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = new ServiceBusSendEndpointProvider(_serializer, InputAddress, _sendTransportProvider, _sendPipe);

            return new SendEndpointCache(provider);
        }

        IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            var provider = new PublishSendEndpointProvider(_serializer, InputAddress, _host);

            return new ServiceBusPublishEndpointProvider(_host, provider, _publishPipe, _publish, _serializer, InputAddress);
        }
    }
}