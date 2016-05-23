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
namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Pipeline;
    using Util;
    using Util.Caching;


    /// <summary>
    /// Caches SendEndpoint instances by address (ignoring the query string entirely, case insensitive)
    /// </summary>
    public class SendEndpointCache :
        ISendEndpointProvider,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<SendEndpointCache>();

        readonly LazyMemoryCache<Uri, ISendEndpoint> _cache;
        readonly Func<Uri, TimeSpan> _cacheDurationProvider;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public SendEndpointCache(ISendEndpointProvider sendEndpointProvider, Func<Uri, TimeSpan> cacheDurationProvider = null)
        {
            _sendEndpointProvider = sendEndpointProvider;

            _cacheDurationProvider = cacheDurationProvider ?? DefaultCacheDurationProvider;

            var cacheId = NewId.NextGuid().ToString();
            _cache = new LazyMemoryCache<Uri, ISendEndpoint>(cacheId, GetSendEndpointFromProvider, GetEndpointCachePolicy, FormatAddressKey,
                OnCachedEndpointRemoved);
        }

        public void Dispose()
        {
            _cache.Dispose();
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            Cached<ISendEndpoint> cached = await _cache.Get(address).ConfigureAwait(false);

            var endpoint = await cached.Value.ConfigureAwait(false);

            return new CachedSendEndpoint(endpoint, cached.Touch);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpointProvider.ConnectSendObserver(observer);
        }

        TimeSpan DefaultCacheDurationProvider(Uri address)
        {
            return TimeSpan.FromMinutes(60);
        }

        Task OnCachedEndpointRemoved(string key, ISendEndpoint value, string reason)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Cached endpoint expired: {0} - {1}", key, reason);

            return TaskUtil.Completed;
        }

        string FormatAddressKey(Uri key)
        {
            return key.ToString().ToLowerInvariant();
        }

        LazyMemoryCache<Uri, ISendEndpoint>.ICacheExpiration GetEndpointCachePolicy(LazyMemoryCache<Uri, ISendEndpoint>.ICacheExpirationSelector selector)
        {
            return selector.SlidingWindow(_cacheDurationProvider(selector.Key));
        }

        Task<ISendEndpoint> GetSendEndpointFromProvider(Uri address)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("GetSendEndpoint: {0}", address);

            return _sendEndpointProvider.GetSendEndpoint(address);
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