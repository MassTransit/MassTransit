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
    using Util.Caching;


    /// <summary>
    /// Caches SendEndpoint instances by address (ignoring the query string entirely, case insensitive)
    /// </summary>
    public class SendEndpointCache<TKey> :
        ISendEndpointCache<TKey>
    {
        readonly IIndex<TKey, CachedSendEndpoint<TKey>> _index;

        public SendEndpointCache()
        {
            var cache = new GreenCache<CachedSendEndpoint<TKey>>(10000, TimeSpan.FromMinutes(1), TimeSpan.FromHours(24), () => DateTime.UtcNow);
            _index = cache.AddIndex("key", x => x.Key);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(TKey key, SendEndpointFactory<TKey> factory)
        {
            CachedSendEndpoint<TKey> sendEndpoint = await _index.Get(key, x => GetSendEndpointFromFactory(x, factory)).ConfigureAwait(false);

            return sendEndpoint;
        }

        async Task<CachedSendEndpoint<TKey>> GetSendEndpointFromFactory(TKey address, SendEndpointFactory<TKey> factory)
        {
            var sendEndpoint = await factory(address).ConfigureAwait(false);

            return new CachedSendEndpoint<TKey>(address, sendEndpoint);
        }
    }
}