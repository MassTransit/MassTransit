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
namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Pipeline;


    public class RequestActivity<TInstance, TData, TRequest, TResponse> :
        Activity<TInstance, TData>
        where TRequest : class
        where TResponse : class
        where TData : class
    {
        readonly Request<TRequest, TResponse> _request;
        readonly Func<ConsumeContext<TData>, TRequest> _requestMessageFactory;

        public RequestActivity(Request<TRequest, TResponse> request, Func<ConsumeContext<TData>, TRequest> requestMessageFactory)
        {
            _request = request;
            _requestMessageFactory = requestMessageFactory;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeContext<TData> consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                throw new ArgumentException("The ConsumeContext was not available");

            ISendEndpoint endpoint = await consumeContext.GetSendEndpoint(_request.Settings.ServiceAddress);

            TRequest requestMessage = _requestMessageFactory(consumeContext);

            var pipe = new SendRequest(consumeContext.ReceiveContext.InputAddress);

            await endpoint.Send(requestMessage, pipe);

            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }


        /// <summary>
        /// Handles the sending of a request to the endpoint specified
        /// </summary>
        class SendRequest :
            IPipe<SendContext<TRequest>>
        {
            readonly Uri _responseAddress;

            public SendRequest(Uri responseAddress)
            {
                _responseAddress = responseAddress;
            }

            public async Task Send(SendContext<TRequest> context)
            {
                context.RequestId = NewId.NextGuid();
                context.ResponseAddress = _responseAddress;
            }

            public bool Visit(IPipeVisitor visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}