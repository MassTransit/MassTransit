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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Integration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Pipeline.Pipes;
    using Pipeline;
    using Specifications;
    using Topology;
    using Topology.Builders;
    using Transports;
    using Util;
    using Util.Caching;


    public class RabbitMqPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly IRabbitMqHost _host;
        readonly IIndex<TypeKey, CachedSendEndpoint<TypeKey>> _index;
        readonly PublishObservable _publishObservable;
        readonly IPublishPipe _publishPipe;
        readonly IRabbitMqTopology _topology;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public RabbitMqPublishEndpointProvider(IRabbitMqHost host, IMessageSerializer serializer, Uri sourceAddress, IPublishPipe publishPipe,
            IRabbitMqTopology topology)
        {
            _host = host;
            _serializer = serializer;
            _sourceAddress = sourceAddress;
            _publishPipe = publishPipe;
            _topology = topology;

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
            IRabbitMqMessagePublishTopologyConfigurator<T> messageTopology = _topology.PublishTopology.GetMessageTopology<T>();

            Uri publishAddress;
            if (!messageTopology.TryGetPublishAddress(_host.Address, message, out publishAddress))
                throw new PublishException($"An address for publishing message type {TypeMetadataCache<T>.ShortName} was not found.");

            return await _index.Get(new TypeKey(typeof(T), publishAddress), typeKey => CreateSendEndpoint<T>(typeKey)).ConfigureAwait(false);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }

        Task<CachedSendEndpoint<TypeKey>> CreateSendEndpoint<T>(TypeKey typeKey)
            where T : class
        {
            IRabbitMqMessagePublishTopologyConfigurator<T> messageTopology = _topology.PublishTopology.GetMessageTopology<T>();

            var sendSettings = messageTopology.GetSendSettings();

            var builder = new PublishEndpointTopologyBuilder();
            messageTopology.ApplyMessageTopology(builder);

            var topology = builder.BuildTopologyLayout();

            var modelCache = new RabbitMqModelCache(_host, _topology);

            var sendTransport = new RabbitMqSendTransport(modelCache, new ConfigureTopologyFilter<SendSettings>(sendSettings, topology), sendSettings.ExchangeName);

            var sendEndpoint = new SendEndpoint(sendTransport, _serializer, typeKey.Address, _sourceAddress, SendPipe.Empty);

            return Task.FromResult(new CachedSendEndpoint<TypeKey>(typeKey, sendEndpoint));
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
    }
}