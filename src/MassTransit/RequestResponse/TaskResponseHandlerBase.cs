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
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using Pipeline;
    using SubscriptionConnectors;

    public abstract class TaskResponseHandlerBase<TResponse> :
        TaskResponseHandler<TResponse>
        where TResponse : class
    {
        protected readonly TaskCompletionSource<TResponse> CompletionSource;
        readonly SynchronizationContext _context;
        readonly HandlerSelector<TResponse> _handler;
        readonly string _requestId;

        protected TaskResponseHandlerBase(string requestId, Action<IConsumeContext<TResponse>, TResponse> handler)
            : this(requestId)
        {
            _handler = HandlerSelector.ForHandler(handler);
        }

        protected TaskResponseHandlerBase(string requestId, Action<TResponse> handler)
            : this(requestId)
        {
            _handler = HandlerSelector.ForHandler(handler);
        }

        TaskResponseHandlerBase(string requestId)
        {
            _requestId = requestId;
            CompletionSource = new TaskCompletionSource<TResponse>(TaskCreationOptions.None);
            _context = SynchronizationContext.Current;
        }

        public Type ResponseType
        {
            get { return typeof(TResponse); }
        }

        public Task Task
        {
            get { return CompletionSource.Task; }
        }

        Task<TResponse> TaskResponseHandler<TResponse>.Task
        {
            get { return CompletionSource.Task; }
        }

        public Task<T> GetTask<T>()
            where T : class
        {
            var self = this as TaskResponseHandlerBase<T>;
            if (self == null)
                throw new InvalidOperationException("An incorrect task type was requested");

            return self.CompletionSource.Task;
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator)
        {
            var connector = new RequestHandlerSubscriptionConnector<TResponse>();

            HandlerSelector<TResponse> handler = HandlerSelector.ForContextHandler<TResponse>(HandleResponse);

            return connector.Connect(configurator, _requestId, handler);
        }

        /// <summary>
        /// Called when the handler completes successfully
        /// </summary>
        /// <param name="context">The message consumer context</param>
        protected abstract void Success(IConsumeContext<TResponse> context);

        /// <summary>
        /// Called when the handler throws an exception (exception is wrapped as a RequestException)
        /// </summary>
        /// <param name="exception">The wrapped exception</param>
        protected abstract void Failure(Exception exception);

        void HandleResponse(IConsumeContext<TResponse> context)
        {
            try
            {
                if (_context != null)
                {
                    _context.Post(state =>
                        {
                            try
                            {
                                Action<IConsumeContext<TResponse>> handler = _handler(context);
                                handler(context);

                                Success(context);
                            }
                            catch (Exception ex)
                            {
                                Failure(context, ex);
                            }
                        }, state: null);
                }
                else
                {
                    Action<IConsumeContext<TResponse>> handler = _handler(context);
                    handler(context);

                    Success(context);
                }
            }
            catch (Exception ex)
            {
                Failure(context, ex);
            }
        }

        void Failure(IConsumeContext<TResponse> context, Exception exception)
        {
            var requestException = new RequestException("The response handler threw an exception", exception,
                context.Message);

            Failure(requestException);
        }
    }

#endif
}