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

    public class TaskResponseWatcherImpl<TResponse> :
        TaskResponseHandler<TResponse>
        where TResponse : class
    {
        readonly string _requestId;
        readonly TaskCompletionSource<TResponse> _source;
        readonly HandlerSelector<TResponse> _watcher;

        public TaskResponseWatcherImpl(string requestId, Action<IConsumeContext<TResponse>, TResponse> handler)
            : this(requestId)
        {
            _watcher = HandlerSelector.ForHandler(handler);
        }

        public TaskResponseWatcherImpl(string requestId, Action<TResponse> handler)
            : this(requestId)
        {
            _watcher = HandlerSelector.ForHandler(handler);
        }

        TaskResponseWatcherImpl(string requestId)
        {
            _requestId = requestId;
            _source = new TaskCompletionSource<TResponse>(TaskCreationOptions.None);
        }

        public Type ResponseType
        {
            get { return typeof(TResponse); }
        }

        Task TaskResponseHandler.Task
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
            throw new InvalidOperationException("Watched types do not have tasks");
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator)
        {
            var connector = new RequestHandlerSubscriptionConnector<TResponse>();

            HandlerSelector<TResponse> watcher = HandlerSelector.ForContextHandler<TResponse>(WatchResponse);

            return connector.Connect(configurator, _requestId, watcher);
        }

        void WatchResponse(IConsumeContext<TResponse> context)
        {
            try
            {
                Action<IConsumeContext<TResponse>> handler = _watcher(context);
                handler(context);
            }
            catch (Exception ex)
            {
                var exception = new RequestException("The response watcher threw an exception", ex, context.Message);
                throw exception;
            }
        }
    }
}