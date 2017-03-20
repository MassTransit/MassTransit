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
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly IPublishPipe _publishPipe;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly ISendTransportProvider _sendTransportProvider;
        readonly IMessageSerializer _serializer;
        readonly IInMemoryEndpointConfiguration _configuration;

        public InMemoryReceiveEndpointTopology(IInMemoryEndpointConfiguration configuration, Uri inputAddress, IMessageSerializer serializer,
            ISendTransportProvider sendTransportProvider)
        {
            InputAddress = inputAddress;
            _configuration = configuration;
            _serializer = serializer;
            _sendTransportProvider = sendTransportProvider;

            Send = configuration.SendTopology;
            Publish = configuration.PublishTopology;

            _sendPipe = configuration.CreateSendPipe();
            _publishPipe = configuration.CreatePublishPipe();

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        public Uri InputAddress { get; }
        public ISendTopology Send { get; }
        public IPublishTopology Publish { get; }

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

            return new InMemoryPublishEndpointProvider(sendEndpointCache, _sendTransportProvider, _publishPipe, _configuration.PublishTopology,
                _serializer, InputAddress);
        }
    }
}