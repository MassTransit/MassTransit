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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Pipeline;
    using Util;


    public abstract class PublishEndpoint :
        IPublishEndpoint,
        IPublishObserver
    {
        readonly Connectable<IPublishObserver> _observers;

        protected PublishEndpoint()
        {
            _observers = new Connectable<IPublishObserver>();
        }

        async Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(this);
            try
            {
                foreach (ISendEndpoint endpoint in await GetEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(message, adapter, cancellationToken).ConfigureAwait(false);

                await adapter.PostSend().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                adapter.SendFaulted(ex).Wait(cancellationToken);
                throw;
            }
        }

        async Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, this);
            try
            {
                foreach (ISendEndpoint endpoint in await GetEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(message, adapter, cancellationToken).ConfigureAwait(false);

                await adapter.PostSend().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                adapter.SendFaulted(ex).Wait(cancellationToken);
                throw;
            }
        }

        async Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, this);
            try
            {
                foreach (ISendEndpoint endpoint in await GetEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(message, adapter, cancellationToken).ConfigureAwait(false);

                await adapter.PostSend().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                adapter.SendFaulted(ex).Wait(cancellationToken);
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
            var adapter = new PublishPipeContextAdapter<T>(this);
            try
            {
                foreach (ISendEndpoint endpoint in await GetEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(values, adapter, cancellationToken).ConfigureAwait(false);

                await adapter.PostSend().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                adapter.SendFaulted(ex).Wait(cancellationToken);
                throw;
            }
        }

        async Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, this);
            try
            {
                foreach (ISendEndpoint endpoint in await GetEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(values, adapter, cancellationToken).ConfigureAwait(false);

                await adapter.PostSend().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                adapter.SendFaulted(ex).Wait(cancellationToken);
                throw;
            }
        }

        async Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            var adapter = new PublishPipeContextAdapter<T>(publishPipe, this);
            try
            {
                foreach (ISendEndpoint endpoint in await GetEndpoints(typeof(T)).ConfigureAwait(false))
                    await endpoint.Send(values, adapter, cancellationToken).ConfigureAwait(false);

                await adapter.PostSend().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                adapter.SendFaulted(ex).Wait(cancellationToken);
                throw;
            }
        }

        public ConnectHandle Connect(IPublishObserver observer)
        {
            return _observers.Connect(observer);
        }

        Task IPublishObserver.PrePublish<T>(PublishContext<T> context)
        {
            return _observers.ForEach(observer => observer.PrePublish(context));
        }

        Task IPublishObserver.PostPublish<T>(PublishContext<T> context)
        {
            return _observers.ForEach(observer => observer.PostPublish(context));
        }

        Task IPublishObserver.PublishFault<T>(PublishContext<T> context, Exception exception)
        {
            return _observers.ForEach(observer => observer.PublishFault(context, exception));
        }

        protected abstract Task<IEnumerable<ISendEndpoint>> GetEndpoints(Type messageType);
    }
}