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
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Pipeline;
    using Util;


    public class PublishEndpoint :
        IPublishEndpoint
    {
        readonly IPublishEndpointProvider _endpointProvider;
        readonly IPublishObserver _publishObserver;
        readonly IPublishPipe _publishPipe;
        readonly Guid? _correlationId;
        readonly Guid? _conversationId;
        readonly Uri _sourceAddress;

        public PublishEndpoint(Uri sourceAddress, IPublishEndpointProvider endpointProvider, IPublishObserver publishObserver, IPublishPipe publishPipe, Guid? correlationId,
            Guid? conversationId)
        {
            _sourceAddress = sourceAddress;
            _endpointProvider = endpointProvider;
            _publishObserver = publishObserver;
            _publishPipe = publishPipe;
            _correlationId = correlationId;
            _conversationId = conversationId;
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(_publishPipe, _publishObserver, _sourceAddress, _correlationId, _conversationId, message);

            return Publish(cancellationToken, message, adapter);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, _publishPipe, _publishObserver, _sourceAddress, _correlationId, _conversationId, message);

            return Publish(cancellationToken, message, adapter);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, _publishPipe, _publishObserver, _sourceAddress, _correlationId, _conversationId, message);

            return Publish(cancellationToken, message, adapter);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            Type messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            Type messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            var adapter = new PublishPipeContextAdapter<T>(_publishPipe, _publishObserver, _sourceAddress, _correlationId, _conversationId, message);

            return Publish(cancellationToken, message, adapter);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            var adapter = new PublishPipeContextAdapter<T>(publishPipe, _publishPipe, _publishObserver, _sourceAddress, _correlationId, _conversationId, message);

            return Publish(cancellationToken, message, adapter);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var message = TypeMetadataCache<T>.InitializeFromObject(values);

            var adapter = new PublishPipeContextAdapter<T>(publishPipe, _publishPipe, _publishObserver, _sourceAddress, _correlationId, _conversationId, message);

            return Publish(cancellationToken, message, adapter);
        }

        async Task Publish<T>(CancellationToken cancellationToken, T message, PublishPipeContextAdapter<T> adapter)
            where T : class
        {
            try
            {
                var sendEndpoint = await _endpointProvider.GetPublishSendEndpoint(message).ConfigureAwait(false);

                await sendEndpoint.Send(message, adapter, cancellationToken).ConfigureAwait(false);

                await adapter.PostPublish().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await adapter.PublishFaulted(ex).ConfigureAwait(false);
                throw;
            }
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _endpointProvider.ConnectPublishObserver(observer);
        }
    }
}