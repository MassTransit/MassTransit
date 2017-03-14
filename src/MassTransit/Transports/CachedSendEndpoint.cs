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
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Util.Caching;


    public class CachedSendEndpoint<TKey> :
        ISendEndpoint,
        INotifyValueUsed
    {
        readonly ISendEndpoint _endpoint;

        public CachedSendEndpoint(TKey key, ISendEndpoint endpoint)
        {
            Key = key;
            _endpoint = endpoint;
        }

        public TKey Key { get; }

        public event Action Used;

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            Used?.Invoke();
            return _endpoint.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            Used?.Invoke();
            return _endpoint.Send(message, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            Used?.Invoke();
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            Used?.Invoke();
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        public Task Send(object message, CancellationToken cancellationToken = new CancellationToken())
        {
            Used?.Invoke();
            return _endpoint.Send(message, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken = new CancellationToken())
        {
            Used?.Invoke();
            return _endpoint.Send(message, messageType, cancellationToken);
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
        {
            Used?.Invoke();
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
        {
            Used?.Invoke();
            return _endpoint.Send(message, messageType, pipe, cancellationToken);
        }

        public Task Send<T>(object values, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            Used?.Invoke();
            return _endpoint.Send<T>(values, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            Used?.Invoke();
            return _endpoint.Send(values, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            Used?.Invoke();
            return _endpoint.Send<T>(values, pipe, cancellationToken);
        }
    }
}