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
    using System;
    using System.Threading.Tasks;
    using Exceptions;
    using Pipeline;
    using SubscriptionConnectors;

    public class TaskResponseHandlerImpl<TResponse> :
        TaskResponseHandler<TResponse>
        where TResponse : class
    {
        readonly HandlerSelector<TResponse> _handler;
        readonly string _requestId;
        readonly TaskCompletionSource<TResponse> _source;

        public TaskResponseHandlerImpl(string requestId, Action<IConsumeContext<TResponse>, TResponse> handler)
            : this(requestId)
        {
            _handler = HandlerSelector.ForHandler(handler);
        }

        public TaskResponseHandlerImpl(string requestId, Action<TResponse> handler)
            : this(requestId)
        {
            _handler = HandlerSelector.ForHandler(handler);
        }

        TaskResponseHandlerImpl(string requestId)
        {
            _requestId = requestId;
            _source = new TaskCompletionSource<TResponse>(TaskCreationOptions.None);
        }

        public Type ResponseType
        {
            get { return typeof(TResponse); }
        }

        public Task Task
        {
            get { return _source.Task; }
        }

        Task<TResponse> TaskResponseHandler<TResponse>.Task
        {
            get { return _source.Task; }
        }

        public Task<T> GetTask<T>()
            where T : class
        {
            var self = this as TaskResponseHandlerImpl<T>;
            if (self == null)
                throw new InvalidOperationException("An incorrect task type was requested");

            return self._source.Task;
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator)
        {
            var connector = new RequestHandlerSubscriptionConnector<TResponse>();

            HandlerSelector<TResponse> handler = HandlerSelector.ForContextHandler<TResponse>(HandleResponse);

            return connector.Connect(configurator, _requestId, handler);
        }

        void HandleResponse(IConsumeContext<TResponse> context)
        {
            try
            {
                Action<IConsumeContext<TResponse>> handler = _handler(context);
                handler(context);

                _source.SetResult(context.Message);
            }
            catch (Exception ex)
            {
                var exception = new RequestException("The response handler threw an exception", ex, context.Message);
                _source.SetException(exception);
            }
        }
    }
}