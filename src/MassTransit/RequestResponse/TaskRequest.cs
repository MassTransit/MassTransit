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
namespace MassTransit.RequestResponse
{
#if NET40
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Advanced;
    using Exceptions;
    using Magnum.Caching;

    public class TaskRequest<TRequest> :
        ITaskRequest<TRequest>
        where TRequest : class
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly TRequest _message;
        readonly string _requestId;

        readonly Cache<Type, TaskResponseHandler> _responseHandlers;
        readonly TaskCompletionSource<TRequest> _source;
        readonly Action _timeoutCallback;

        TimeSpan _timeout;
        Task _timeoutTask;
        UnsubscribeAction _unsubscribe;

        public TaskRequest(string requestId, TRequest message, TimeSpan timeout, Action timeoutCallback, IServiceBus bus,
            IEnumerable<TaskResponseHandler> handlers)
        {
            _requestId = requestId;
            _message = message;
            _timeout = timeout;
            _timeoutCallback = timeoutCallback;
            _cancellationTokenSource = new CancellationTokenSource();

            _source = new TaskCompletionSource<TRequest>(TaskCreationOptions.None);
            _source.Task.ContinueWith(_ => { _unsubscribe(); });

            _responseHandlers = new DictionaryCache<Type, TaskResponseHandler>(x => x.ResponseType, handlers);

            _unsubscribe = SubscribeHandlers(bus);

            if (timeout > TimeSpan.Zero)
            {
                _timeoutTask = TaskHelper.Timeout(timeout, _cancellationTokenSource.Token).ContinueWith(HandleTimeout);
            }
        }

        public string RequestId
        {
            get { return _requestId; }
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
            _source.TrySetCanceled();

            Cleanup();
        }

        public TRequest RequestMessage
        {
            get { return _message; }
        }

        public Task Task
        {
            get { return _source.Task; }
        }

        public Task<T> GetResponseTask<T>()
            where T : class
        {
            if (_responseHandlers.Has(typeof(T)))
            {
                TaskResponseHandler handler = _responseHandlers[typeof(T)];

                return handler.GetTask<T>();
            }

            throw new InvalidOperationException("The response task was not found.");
        }

        UnsubscribeAction SubscribeHandlers(IServiceBus bus)
        {
            foreach (TaskResponseHandler handler in _responseHandlers)
            {
                handler.Task.ContinueWith(_ =>
                    {
                        _source.TrySetResult(_message);
                        Cleanup();
                    });
            }

            return bus.InboundPipeline
                .Configure(configurator => _responseHandlers.CombineSubscriptions(x => x.Connect(configurator)));
        }

        void HandleTimeout(Task task)
        {
            try
            {
                if (_timeoutCallback != null)
                {
                    _timeoutCallback();
                }
                else
                {
                    RequestTimeoutException exception = RequestTimeoutException.FromCorrelationId(_requestId);
                    _source.TrySetException(exception);
                }
            }
            finally
            {
                Cleanup();
            }
        }

        void Cleanup()
        {
            _unsubscribe();
            _unsubscribe = () => false;
        }
    }
#endif
}