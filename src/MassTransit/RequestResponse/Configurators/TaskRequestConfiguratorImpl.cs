// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RequestResponse.Configurators
{
#if NET40
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using Magnum.Caching;
    using Magnum.Extensions;

    public class TaskRequestConfiguratorImpl<TRequest> :
        TaskRequestConfigurator<TRequest>
        where TRequest : class
    {
        readonly Cache<Type, TaskResponseHandler> _handlers;
        readonly TRequest _message;
        readonly string _requestId;
        TimeSpan _expiration;
        TimeSpan _timeout;
        Action _timeoutCallback;

        public TaskRequestConfiguratorImpl(TRequest message)
        {
            _message = message;
            _requestId = NewId.NextGuid().ToString();

            _timeout = TimeSpan.Zero;
            _expiration = TimeSpan.MaxValue;
            _handlers = new DictionaryCache<Type, TaskResponseHandler>();
        }

        public string RequestId
        {
            get { return _requestId; }
        }

        public void HandleTimeout(TimeSpan timeout, Action timeoutCallback)
        {
            _timeout = timeout;
            _timeoutCallback = timeoutCallback;
        }

        public void SetTimeout(TimeSpan timeout)
        {
            _timeout = timeout;
            _timeoutCallback = () => { throw RequestTimeoutException.FromCorrelationId(_requestId); };
        }

        public void SetRequestExpiration(TimeSpan expiration)
        {
            _expiration = expiration;
        }

        public TRequest Request
        {
            get { return _message; }
        }

        public Task<TResponse> Handle<TResponse>(Action<TResponse> handler)
            where TResponse : class
        {
            return AddHandler(() => new TaskResponseHandlerImpl<TResponse>(_requestId, handler));
        }

        public Task<TResponse> Handle<TResponse>(Action<IConsumeContext<TResponse>, TResponse> handler)
            where TResponse : class
        {
            return AddHandler(() => new TaskResponseHandlerImpl<TResponse>(_requestId, handler));
        }

        public void Watch<T>(Action<T> watcher)
            where T : class
        {
            AddWatcher<T>(() => new TaskResponseWatcherImpl<T>(_requestId, watcher));
        }

        public void Watch<T>(Action<IConsumeContext<T>, T> watcher)
            where T : class
        {
            AddWatcher<T>(() => new TaskResponseWatcherImpl<T>(_requestId, watcher));
        }

        Task<TResponse> AddHandler<TResponse>(Func<TaskResponseHandlerImpl<TResponse>> responseHandlerFactory)
            where TResponse : class
        {
            if (_handlers.Has(typeof(TResponse)))
                throw new ArgumentException("A response handler for {0} has already been declared."
                    .FormatWith(typeof(TResponse).Name));

            TaskResponseHandler<TResponse> responseHandler = responseHandlerFactory();

            _handlers.Add(typeof(TResponse), responseHandler);

            return responseHandler.Task;
        }

        void AddWatcher<TResponse>(Func<TaskResponseHandler> responseHandlerFactory)
            where TResponse : class
        {
            if (_handlers.Has(typeof(TResponse)))
                throw new ArgumentException("A response handler for {0} has already been declared."
                    .FormatWith(typeof(TResponse).Name));

            TaskResponseHandler responseHandler = responseHandlerFactory();

            _handlers.Add(typeof(TResponse), responseHandler);
        }

        public ITaskRequest<TRequest> Create(IServiceBus bus)
        {
            var request = new TaskRequest<TRequest>(_requestId, _message, _timeout, _timeoutCallback, bus, _handlers);

            return request;
        }
    }
#endif
}