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
namespace MassTransit.Transports.InMemory
{
    using System;
    using MassTransit.Topology;
    using Pipeline;
    using Pipeline.Pipes;


    public class InMemoryReceiveEndpointTopology :
        IInMemoryReceiveEndpointTopology
    {
        readonly IInMemoryEndpointConfiguration _configuration;
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly IPublishPipe _publishPipe;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly IMessageSerializer _serializer;

        public InMemoryReceiveEndpointTopology(IInMemoryEndpointConfiguration configuration, Uri inputAddress, IMessageSerializer serializer,
            ISendTransportProvider sendTransportProvider)
        {
            InputAddress = inputAddress;
            _configuration = configuration;
            _serializer = serializer;
            _sendTransportProvider = sendTransportProvider;

            _sendPipe = configuration.Send.CreatePipe();
            _publishPipe = configuration.Publish.CreatePipe();

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        public Uri InputAddress { get; }
        public ISendTopology Send => _configuration.Topology.Send;
        public IPublishTopology Publish => _configuration.Topology.Publish;

        ISendEndpointProvider IReceiveEndpointTopology.SendEndpointProvider => _sendEndpointProvider.Value;
        IPublishEndpointProvider IReceiveEndpointTopology.PublishEndpointProvider => _publishEndpointProvider.Value;
        ISendTransportProvider IReceiveEndpointTopology.SendTransportProvider => _sendTransportProvider;

        ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = new InMemorySendEndpointProvider(InputAddress, _sendTransportProvider, _serializer, _sendPipe);

            return new SendEndpointCache(provider);
        }

        IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            var sendEndpointProvider = new InMemorySendEndpointProvider(InputAddress, _sendTransportProvider, _serializer, SendPipe.Empty);

            var sendEndpointCache = new SendEndpointCache(sendEndpointProvider);

            return new InMemoryPublishEndpointProvider(_sendTransportProvider, _publishPipe, _configuration.Topology.Publish, _serializer, InputAddress);
        }
    }
}