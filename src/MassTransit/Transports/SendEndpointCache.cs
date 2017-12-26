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
namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using Util.Caching;


    public delegate Task<ISendEndpoint> SendEndpointFactory(Uri address);


    public interface ISendEndpointCache
    {
        /// <summary>
        /// Return a SendEndpoint from the cache, using the factory to create it if it doesn't exist in the cache.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        Task<ISendEndpoint> GetSendEndpoint(Uri address, SendEndpointFactory factory);
    }


    /// <summary>
    /// Caches SendEndpoint instances by address (ignoring the query string entirely, case insensitive)
    /// </summary>
    public class SendEndpointCache :
        ISendEndpointProvider,
        ISendEndpointCache
    {
        static readonly ILog _log = Logger.Get<SendEndpointCache>();

        readonly IIndex<Uri, CachedSendEndpoint<Uri>> _index;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public SendEndpointCache(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;

            var cache = new GreenCache<CachedSendEndpoint<Uri>>(10000, TimeSpan.FromMinutes(1), TimeSpan.FromHours(24), () => DateTime.UtcNow);
            _index = cache.AddIndex("address", x => x.Key);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address, SendEndpointFactory factory)
        {
            CachedSendEndpoint<Uri> sendEndpoint = await _index.Get(address, x => GetSendEndpointFromFactory(x, factory)).ConfigureAwait(false);

            return sendEndpoint;
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            CachedSendEndpoint<Uri> sendEndpoint = await _index.Get(address, GetSendEndpointFromProvider).ConfigureAwait(false);

            return sendEndpoint;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpointProvider.ConnectSendObserver(observer);
        }

        async Task<CachedSendEndpoint<Uri>> GetSendEndpointFromProvider(Uri address)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("GetSendEndpoint: {0}", address);

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);

            return new CachedSendEndpoint<Uri>(address, sendEndpoint);
        }

        async Task<CachedSendEndpoint<Uri>> GetSendEndpointFromFactory(Uri address, SendEndpointFactory factory)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("GetSendEndpoint (factory): {0}", address);

            var sendEndpoint = await factory(address).ConfigureAwait(false);

            return new CachedSendEndpoint<Uri>(address, sendEndpoint);
        }
    }
}