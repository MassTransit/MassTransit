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
namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class InMemoryPublishSendEndpointProvider :
        IPublishSendEndpointProvider
    {
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly InMemoryTransportCache _transportCache;

        public InMemoryPublishSendEndpointProvider(ISendEndpointProvider sendEndpointProvider, ISendTransportProvider transportProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _transportCache = transportProvider as InMemoryTransportCache;
        }

        public async Task<IEnumerable<ISendEndpoint>> GetPublishEndpoints(Type messageType)
        {
            var endpoints = new List<ISendEndpoint>();
            foreach (Uri transport in _transportCache.TransportAddresses)
                endpoints.Add(await _sendEndpointProvider.GetSendEndpoint(transport).ConfigureAwait(false));

            return endpoints;
        }
    }
}