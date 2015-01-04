// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Pipeline;


    public class InMemoryPublishEndpoint :
        IPublishEndpoint
    {
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly InMemoryTransportCache _transportCache;

        public InMemoryPublishEndpoint(ISendEndpointProvider sendEndpointProvider, ISendTransportProvider transportProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _transportCache = transportProvider as InMemoryTransportCache;
        }

        async Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            foreach (ISendEndpoint endpoint in await GetEndpoints())
                await endpoint.Send(message, cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            foreach (ISendEndpoint endpoint in await GetEndpoints())
                await endpoint.Send(message, new PublishPipeContextAdapter<T>(publishPipe), cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            foreach (ISendEndpoint endpoint in await GetEndpoints())
                await endpoint.Send(message, new PublishPipeContextAdapter(publishPipe), cancellationToken);
        }

        async Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            foreach (ISendEndpoint endpoint in await GetEndpoints())
                await endpoint.Send(message, cancellationToken);
        }

        async Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            foreach (ISendEndpoint endpoint in await GetEndpoints())
                await endpoint.Send(message, new PublishPipeContextAdapter(publishPipe), cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            foreach (ISendEndpoint endpoint in await GetEndpoints())
                await endpoint.Send<T>(values, cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            foreach (ISendEndpoint endpoint in await GetEndpoints())
                await endpoint.Send<T>(values, new PublishPipeContextAdapter<T>(publishPipe), cancellationToken);
        }

        async Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            foreach (ISendEndpoint endpoint in await GetEndpoints())
                await endpoint.Send<T>(values, new PublishPipeContextAdapter(publishPipe), cancellationToken);
        }

        async Task<IEnumerable<ISendEndpoint>> GetEndpoints()
        {
            var endpoints = new List<ISendEndpoint>();
            foreach (Uri transport in _transportCache.TransportAddresses)
                endpoints.Add(await _sendEndpointProvider.GetSendEndpoint(transport));

            return endpoints;
        }
    }
}