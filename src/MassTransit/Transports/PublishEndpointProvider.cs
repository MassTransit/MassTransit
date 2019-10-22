// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;
    using Pipeline;
    using Pipeline.Observables;
    using Pipeline.Pipes;
    using Topology;
    using Util;


    public class PublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly ISendEndpointCache<Type> _cache;
        readonly Uri _hostAddress;
        readonly PublishObservable _publishObservers;
        readonly IPublishPipe _publishPipe;
        readonly IPublishTopology _publishTopology;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;
        readonly IPublishTransportProvider _transportProvider;

        public PublishEndpointProvider(IPublishTransportProvider transportProvider, Uri hostAddress, PublishObservable publishObservers,
            IMessageSerializer serializer, Uri sourceAddress, IPublishPipe publishPipe, IPublishTopology publishTopology)
        {
            _transportProvider = transportProvider;
            _hostAddress = hostAddress;
            _serializer = serializer;
            _sourceAddress = sourceAddress;
            _publishPipe = publishPipe;
            _publishTopology = publishTopology;

            _publishObservers = publishObservers;

            _cache = new SendEndpointCache<Type>();
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, ConsumeContext consumeContext)
        {
            return new PublishEndpoint(sourceAddress, this, _publishObservers, _publishPipe, consumeContext);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>(T message)
            where T : class
        {
            return _cache.GetSendEndpoint(typeof(T), type => CreateSendEndpoint<T>());
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservers.Connect(observer);
        }

        async Task<ISendEndpoint> CreateSendEndpoint<T>()
            where T : class
        {
            IMessagePublishTopology<T> messageTopology = _publishTopology.GetMessageTopology<T>();

            if (!messageTopology.TryGetPublishAddress(_hostAddress, out var publishAddress))
                throw new PublishException($"An address for publishing message type {TypeMetadataCache<T>.ShortName} was not found.");

            var sendTransport = await _transportProvider.GetPublishTransport<T>(publishAddress).ConfigureAwait(false);

            return new SendEndpoint(sendTransport, _serializer, publishAddress, _sourceAddress, SendPipe.Empty);
        }
    }
}