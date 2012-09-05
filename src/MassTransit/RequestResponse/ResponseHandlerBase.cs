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
    using System.Threading;
    using Exceptions;
    using Pipeline;
    using SubscriptionConnectors;

    public abstract class ResponseHandlerBase<TResponse> :
        ResponseHandler
        where TResponse : class
    {
        readonly HandlerSelector<TResponse> _handler;
        readonly string _requestId;
        readonly SynchronizationContext _synchronizationContext;

        protected ResponseHandlerBase(string requestId, SynchronizationContext synchronizationContext,
            Action<IConsumeContext<TResponse>, TResponse> handler)
            : this(requestId, synchronizationContext)
        {
            _handler = HandlerSelector.ForHandler(handler);
        }

        protected ResponseHandlerBase(string requestId, SynchronizationContext synchronizationContext,
            Action<TResponse> handler)
            : this(requestId, synchronizationContext)
        {
            _handler = HandlerSelector.ForHandler(handler);
        }

        ResponseHandlerBase(string requestId, SynchronizationContext synchronizationContext)
        {
            _requestId = requestId;
            _synchronizationContext = synchronizationContext;
        }

        Type ResponseHandler.ResponseType
        {
            get { return typeof(TResponse); }
        }

        UnsubscribeAction ResponseHandler.Connect(IInboundPipelineConfigurator configurator)
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
                if (_synchronizationContext != null)
                {
                    _synchronizationContext.Post(state =>
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
}