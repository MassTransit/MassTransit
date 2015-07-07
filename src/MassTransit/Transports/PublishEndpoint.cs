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
    using Pipeline;


    public class PublishEndpoint :
        IPublishEndpoint
    {
        readonly IPublishSendEndpointProvider _endpointProvider;
        readonly PublishObservable _observers;
        readonly Uri _sourceAddress;

        public PublishEndpoint(Uri sourceAddress, IPublishSendEndpointProvider endpointProvider)
        {
            _sourceAddress = sourceAddress;
            _endpointProvider = endpointProvider;
            _observers = new PublishObservable();
        }

        async Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(_observers, _sourceAddress);
            try
            {
                foreach (ISendEndpoint endpoint in await _endpointProvider.GetPublishEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(message, adapter, cancellationToken).ConfigureAwait(false);

                adapter.PostPublish();
            }
            catch (Exception ex)
            {
                adapter.PublishFaulted(ex);
                throw;
            }
        }

        async Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, _observers, _sourceAddress);
            try
            {
                foreach (ISendEndpoint endpoint in await _endpointProvider.GetPublishEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(message, adapter, cancellationToken).ConfigureAwait(false);

                adapter.PostPublish();
            }
            catch (Exception ex)
            {
                adapter.PublishFaulted(ex);
                throw;
            }
        }

        async Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, _observers, _sourceAddress);
            try
            {
                foreach (ISendEndpoint endpoint in await _endpointProvider.GetPublishEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(message, adapter, cancellationToken).ConfigureAwait(false);

                adapter.PostPublish();
            }
            catch (Exception ex)
            {
                adapter.PublishFaulted(ex);
                throw;
            }
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Type messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");

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

        async Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(_observers, _sourceAddress);
            try
            {
                foreach (ISendEndpoint endpoint in await _endpointProvider.GetPublishEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(values, adapter, cancellationToken).ConfigureAwait(false);

                adapter.PostPublish();
            }
            catch (Exception ex)
            {
                adapter.PublishFaulted(ex);
                throw;
            }
        }

        async Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, _observers, _sourceAddress);
            try
            {
                foreach (ISendEndpoint endpoint in await _endpointProvider.GetPublishEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(values, adapter, cancellationToken).ConfigureAwait(false);

                adapter.PostPublish();
            }
            catch (Exception ex)
            {
                adapter.PublishFaulted(ex);
                throw;
            }
        }

        async Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, _observers, _sourceAddress);
            try
            {
                foreach (ISendEndpoint endpoint in await _endpointProvider.GetPublishEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(values, adapter, cancellationToken).ConfigureAwait(false);

                adapter.PostPublish();
            }
            catch (Exception ex)
            {
                adapter.PublishFaulted(ex);
                throw;
            }
        }

        public ConnectHandle Connect(IPublishObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}