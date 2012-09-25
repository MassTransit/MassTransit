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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Advanced;
    using Exceptions;
    using Magnum.Caching;
    using Magnum.Extensions;

    public class TaskRequest<TRequest> :
        ITaskRequest<TRequest>
        where TRequest : class
    {
        readonly CancellationTokenSource _cancelRequest;
        readonly CancellationTokenSource _cancelTimeout;
        readonly TRequest _message;
        readonly TimeoutHandler<TRequest> _timeoutHandler;
        readonly string _requestId;
        readonly Cache<Type, TaskResponseHandler> _responseHandlers;
        readonly TaskCompletionSource<TRequest> _source;

        CancellationTokenRegistration _cancelRequestRegistration;
        UnsubscribeAction _unsubscribe;

        public TaskRequest(string requestId, TRequest message, TimeSpan timeout, TimeoutHandler<TRequest> timeoutHandler,
            CancellationToken cancellationToken, IServiceBus bus, IEnumerable<ResponseHandler> handlers)
        {
            _requestId = requestId;
            _message = message;
            _timeoutHandler = timeoutHandler;
            _responseHandlers = new DictionaryCache<Type, TaskResponseHandler>(x => x.ResponseType,
                handlers.Cast<TaskResponseHandler>());

            // this is our task, which we complete/fail/cancel as appropriate
            _source = new TaskCompletionSource<TRequest>(TaskCreationOptions.None);
            _source.Task.ContinueWith(HandleCompletion);

            // this is what gets called to cancel the request, cancels the task as well
            _cancelRequest = new CancellationTokenSource();
            _cancelRequestRegistration = cancellationToken.Register(HandleCancel);

            // this is what we call to cancel the timeout if the request is cancelled or a handler completes
            _cancelTimeout = new CancellationTokenSource();
            if (timeout >= TimeSpan.Zero)
            {
                Task timeoutTask = TaskHelper.Timeout(timeout, _cancelTimeout.Token);
                timeoutTask.ContinueWith(HandleTimeout, TaskContinuationOptions.NotOnCanceled);
            }

            _unsubscribe = SubscribeHandlers(bus);
        }

        public string RequestId
        {
            get { return _requestId; }
        }

        public TRequest Message
        {
            get { return _message; }
        }

        public void Cancel()
        {
            _cancelRequest.Cancel();
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

            throw new ArgumentException("A response handler for the specified type was not found: "
                                        + typeof(T).ToShortTypeName());
        }

        public Task GetResponseTask(Type responseType)
        {
            if (_responseHandlers.Has(responseType))
                return _responseHandlers[responseType].Task;

            throw new ArgumentException("A response handler for the specified type was not found: "
                                        + responseType.ToShortTypeName());
        }

        void HandleCompletion(Task<TRequest> task)
        {
            try
            {
                _cancelTimeout.Cancel();
            }
            finally
            {
                Cleanup();
            }
        }

        void HandleTimeout(Task task)
        {
            try
            {
                if(_timeoutHandler != null)
                {
                    _timeoutHandler.HandleTimeout(_message);
                    _source.TrySetResult(_message);
                }
                else
                {
                    var exception = new RequestTimeoutException(_requestId);
                    _source.TrySetException(exception);
                }

                NotifyHandlersOfTimeout();
            }
            finally
            {
                Cleanup();
            }
        }

        void NotifyHandlersOfTimeout()
        {
            foreach (TaskResponseHandler handler in _responseHandlers)
            {
                handler.HandleTimeout();
            }
        }

        void HandleCancel()
        {
            if (_source.Task.IsCompleted || _source.Task.IsFaulted)
                return;

            try
            {
                _source.TrySetCanceled();
            }
            catch (Exception ex)
            {
                _source.TrySetException(ex);
            }
            finally
            {
                Cleanup();
            }
        }

        UnsubscribeAction SubscribeHandlers(IServiceBus bus)
        {
            foreach (TaskResponseHandler handler in _responseHandlers)
            {
                handler.Task.ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                            _source.TrySetException(x.Exception);
                        else
                            _source.TrySetResult(_message);
                    });
            }

            return bus.InboundPipeline
                .Configure(x => _responseHandlers.CombineSubscriptions(h => h.Connect(x)));
        }

        void Cleanup()
        {
            _cancelRequestRegistration.Dispose();

            _unsubscribe();
            _unsubscribe = () => false;
        }
    }
#endif
}