// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Integration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using Topology;
    using Transports;
    using Util;
    using Util.Caching;


    public class RabbitMqPublishEndpointProvider :
        IPublishEndpointProvider,
        IDisposable
    {
        readonly LazyMemoryCache<Type, ISendEndpoint> _cache;
        readonly IRabbitMqHost _host;
        readonly ModelSettings _modelSettings;
        readonly PublishObservable _publishObservable;
        readonly IPublishPipe _publishPipe;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public RabbitMqPublishEndpointProvider(IRabbitMqHost host, IMessageSerializer serializer, Uri sourceAddress, IPublishPipe publishPipe,
            ModelSettings modelSettings)
        {
            _host = host;
            _serializer = serializer;
            _sourceAddress = sourceAddress;
            _publishPipe = publishPipe;
            _modelSettings = modelSettings;

            _publishObservable = new PublishObservable();

            var cacheId = NewId.NextGuid().ToString();
            _cache = new LazyMemoryCache<Type, ISendEndpoint>(cacheId, CreateSendEndpoint, GetEndpointCachePolicy, FormatAddressKey,
                OnCachedEndpointRemoved);
        }

        public void Dispose()
        {
            _cache.Dispose();
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, Guid? correlationId, Guid? conversationId)
        {
            return new PublishEndpoint(sourceAddress, this, _publishObservable, _publishPipe, correlationId, conversationId);
        }

        public async Task<ISendEndpoint> GetPublishSendEndpoint(Type messageType)
        {
            Cached<ISendEndpoint> cached = await _cache.Get(messageType).ConfigureAwait(false);

            var endpoint = await cached.Value.ConfigureAwait(false);

            return new CachedSendEndpoint(endpoint, cached.Touch);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }

        Task OnCachedEndpointRemoved(string key, ISendEndpoint value, string reason)
        {
            return TaskUtil.Completed;
        }

        string FormatAddressKey(Type key)
        {
            return TypeMetadataCache.GetShortName(key);
        }

        LazyMemoryCache<Type, ISendEndpoint>.ICacheExpiration GetEndpointCachePolicy(LazyMemoryCache<Type, ISendEndpoint>.ICacheExpirationSelector selector)
        {
            return selector.SlidingWindow(TimeSpan.FromDays(1));
        }

        Task<ISendEndpoint> CreateSendEndpoint(Type messageType)
        {
            var sendSettings = _host.Settings.GetSendSettings(messageType);

            ExchangeBindingSettings[] bindings = TypeMetadataCache.GetMessageTypes(messageType)
                .SelectMany(type => type.GetExchangeBindings(_host.Settings.MessageNameFormatter))
                .Where(binding => !sendSettings.ExchangeName.Equals(binding.Exchange.ExchangeName))
                .ToArray();

            var destinationAddress = _host.Settings.GetSendAddress(sendSettings);

            var modelCache = new RabbitMqModelCache(_host.ConnectionCache, _host.Supervisor, _modelSettings);

            var sendTransport = new RabbitMqSendTransport(modelCache, sendSettings, bindings);

            return Task.FromResult<ISendEndpoint>(new SendEndpoint(sendTransport, _serializer, destinationAddress, _sourceAddress, SendPipe.Empty));
        }


        class CachedSendEndpoint :
            ISendEndpoint
        {
            readonly ISendEndpoint _endpoint;
            readonly Action _touch;

            public CachedSendEndpoint(ISendEndpoint endpoint, Action touch)
            {
                _endpoint = endpoint;
                _touch = touch;
            }

            public ConnectHandle ConnectSendObserver(ISendObserver observer)
            {
                return _endpoint.ConnectSendObserver(observer);
            }

            public Task Send<T>(T message, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                _touch();
                return _endpoint.Send(message, cancellationToken);
            }

            public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                _touch();
                return _endpoint.Send(message, pipe, cancellationToken);
            }

            public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                _touch();
                return _endpoint.Send(message, pipe, cancellationToken);
            }

            public Task Send(object message, CancellationToken cancellationToken = new CancellationToken())
            {
                _touch();
                return _endpoint.Send(message, cancellationToken);
            }

            public Task Send(object message, Type messageType, CancellationToken cancellationToken = new CancellationToken())
            {
                _touch();
                return _endpoint.Send(message, messageType, cancellationToken);
            }

            public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
            {
                _touch();
                return _endpoint.Send(message, pipe, cancellationToken);
            }

            public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
            {
                _touch();
                return _endpoint.Send(message, messageType, pipe, cancellationToken);
            }

            public Task Send<T>(object values, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                _touch();
                return _endpoint.Send<T>(values, cancellationToken);
            }

            public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                _touch();
                return _endpoint.Send(values, pipe, cancellationToken);
            }

            public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
            {
                _touch();
                return _endpoint.Send<T>(values, pipe, cancellationToken);
            }
        }
    }
}