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
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;
    using Pipeline.Pipes;
    using Topology;
    using Util;
    using Util.Caching;


    public class InMemoryPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly InMemoryHost _host;
        readonly IIndex<TypeKey, CachedSendEndpoint<TypeKey>> _index;
        readonly PublishObservable _publishObservable;
        readonly IPublishPipe _publishPipe;
        readonly IInMemoryPublishTopology _publishTopology;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public InMemoryPublishEndpointProvider(ISendTransportProvider transportProvider, IPublishPipe publishPipe, IInMemoryPublishTopology publishTopology,
            IMessageSerializer serializer, Uri sourceAddress)
        {
            _publishPipe = publishPipe;
            _publishTopology = publishTopology;
            _serializer = serializer;
            _sourceAddress = sourceAddress;
            _host = transportProvider as InMemoryHost;
            _publishObservable = new PublishObservable();

            var cache = new GreenCache<CachedSendEndpoint<TypeKey>>(10000, TimeSpan.FromMinutes(1), TimeSpan.FromHours(24), () => DateTime.UtcNow);
            _index = cache.AddIndex("type", x => x.Key);
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, Guid? correlationId, Guid? conversationId)
        {
            return new PublishEndpoint(sourceAddress, this, _publishObservable, _publishPipe, correlationId, conversationId);
        }

        public async Task<ISendEndpoint> GetPublishSendEndpoint<T>(T message)
            where T : class
        {
            if (!_publishTopology.GetMessageTopology<T>().TryGetPublishAddress(_host.Address, out var publishAddress))
                throw new PublishException($"An address for publishing message type {TypeMetadataCache<T>.ShortName} was not found.");

            return await _index.Get(new TypeKey(typeof(T), publishAddress), typeKey => CreateSendEndpoint<T>(typeKey)).ConfigureAwait(false);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }

        async Task<CachedSendEndpoint<TypeKey>> CreateSendEndpoint<T>(TypeKey typeKey)
            where T : class
        {
            var builder = _host.CreatePublishTopologyBuilder();

            _publishTopology.GetMessageTopology<T>().Apply(builder);

            ISendTransport exchange = await _host.GetSendTransport(typeKey.Address).ConfigureAwait(false);

            var sendEndpoint = new SendEndpoint(exchange, _serializer, typeKey.Address, _sourceAddress, SendPipe.Empty);

            return new CachedSendEndpoint<TypeKey>(typeKey, sendEndpoint);
        }


        struct TypeKey
        {
            public readonly Type MessageType;
            public readonly Uri Address;

            public TypeKey(Type messageType, Uri address)
            {
                MessageType = messageType;
                Address = address;
            }

            public bool Equals(TypeKey other)
            {
                return MessageType == other.MessageType && Address.Equals(other.Address);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                return obj is TypeKey && Equals((TypeKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (MessageType.GetHashCode() * 397) ^ Address.GetHashCode();
                }
            }
        }


        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return new EmptyConnectHandle();
        }
    }
}