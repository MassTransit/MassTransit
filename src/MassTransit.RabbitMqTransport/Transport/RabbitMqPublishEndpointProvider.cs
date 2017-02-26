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
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Integration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using MassTransit.Pipeline.Pipes;
    using Topology;
    using Transports;
    using Util;
    using Util.Caching;


    public class RabbitMqPublishEndpointProvider :
        IPublishEndpointProvider
    {
        readonly IRabbitMqHost _host;
        readonly IIndex<Type, CachedSendEndpoint<Type>> _index;
        readonly PublishObservable _publishObservable;
        readonly IPublishPipe _publishPipe;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public RabbitMqPublishEndpointProvider(IRabbitMqHost host, IMessageSerializer serializer, Uri sourceAddress, IPublishPipe publishPipe)
        {
            _host = host;
            _serializer = serializer;
            _sourceAddress = sourceAddress;
            _publishPipe = publishPipe;

            _publishObservable = new PublishObservable();

            var cache = new GreenCache<CachedSendEndpoint<Type>>(10000, TimeSpan.FromMinutes(1), TimeSpan.FromHours(24), () => DateTime.UtcNow);
            _index = cache.AddIndex("type", x => x.Key);
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, Guid? correlationId, Guid? conversationId)
        {
            return new PublishEndpoint(sourceAddress, this, _publishObservable, _publishPipe, correlationId, conversationId);
        }

        public async Task<ISendEndpoint> GetPublishSendEndpoint(Type messageType)
        {
            CachedSendEndpoint<Type> sendEndpoint = await _index.Get(messageType, CreateSendEndpoint).ConfigureAwait(false);

            return sendEndpoint;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }

        Task<CachedSendEndpoint<Type>> CreateSendEndpoint(Type messageType)
        {
            if (!TypeMetadataCache.IsValidMessageType(messageType))
                throw new MessageException(messageType, "Anonymous types are not valid message types");

            var sendSettings = _host.Settings.GetSendSettings(messageType);

            ExchangeBindingSettings[] bindings = TypeMetadataCache.GetMessageTypes(messageType)
                .SelectMany(type => type.GetExchangeBindings(_host.Settings.MessageNameFormatter))
                .Where(binding => !sendSettings.ExchangeName.Equals(binding.Exchange.ExchangeName))
                .ToArray();

            var destinationAddress = sendSettings.GetSendAddress(_host.Settings.HostAddress);

            var modelCache = new RabbitMqModelCache(_host);

            var sendTransport = new RabbitMqSendTransport(modelCache, sendSettings, bindings);

            var sendEndpoint = new SendEndpoint(sendTransport, _serializer, destinationAddress, _sourceAddress, SendPipe.Empty);

            return Task.FromResult(new CachedSendEndpoint<Type>(messageType, sendEndpoint));
        }
    }
}