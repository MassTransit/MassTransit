// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using Pipeline;
    using Util;


    /// <summary>
    /// Caches SendEndpoint instances by address (ignoring the query string entirely, case insensitive)
    /// </summary>
    public class SendEndpointCache :
        ISendEndpointProvider
    {
        static readonly ILog _log = Logger.Get<SendEndpointCache>();

        readonly LazyConcurrentDictionary<Uri, ISendEndpoint> _cache;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public SendEndpointCache(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _cache = new LazyConcurrentDictionary<Uri, ISendEndpoint>(GetSendEndpointFromProvider, AddressEqualityComparer.Comparer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _cache.Get(address);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpointProvider.ConnectSendObserver(observer);
        }

        Task<ISendEndpoint> GetSendEndpointFromProvider(Uri address)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("GetSendEndpoint: {0}", address);

            return _sendEndpointProvider.GetSendEndpoint(address);
        }
    }
}