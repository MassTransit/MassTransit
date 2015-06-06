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
    using Events;
    using MassTransit;
    using MassTransit.Pipeline;


    public class RequestActivity<TInstance, TData, TRequest, TResponse> :
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
        where TRequest : class
        where TResponse : class
    {
        readonly Request<TInstance, TRequest, TResponse> _request;
        readonly Func<ConsumeContext<TData>, TRequest> _requestMessageFactory;

        public RequestActivity(Request<TInstance, TRequest, TResponse> request, Func<ConsumeContext<TData>, TRequest> requestMessageFactory)
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

            TRequest requestMessage = _requestMessageFactory(consumeContext);

            var pipe = new SendRequestPipe(consumeContext.ReceiveContext.InputAddress);

            ISendEndpoint endpoint = await consumeContext.GetSendEndpoint(_request.Settings.ServiceAddress);

            Task sendTask = endpoint.Send(requestMessage, pipe);

            _request.SetRequestId(context.Instance, pipe.RequestId);

            if (_request.Settings.SchedulingServiceAddress != null)
            {
                ISendEndpoint scheduleEndpoint = await consumeContext.GetSendEndpoint(_request.Settings.SchedulingServiceAddress);
                DateTime now = DateTime.UtcNow;
                DateTime expirationTime = now + _request.Settings.Timeout;

                RequestTimeoutExpired message = new TimeoutExpired(now, expirationTime, context.Instance.CorrelationId, pipe.RequestId);

                await scheduleEndpoint.ScheduleSend(consumeContext.ReceiveContext.InputAddress, expirationTime, message);
            }

            await sendTask;

            await next.Execute(context);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }


        /// <summary>
        /// Handles the sending of a request to the endpoint specified
        /// </summary>
        class SendRequestPipe :
            IPipe<SendContext<TRequest>>
        {
            readonly Uri _responseAddress;
            Guid _requestId;

            public SendRequestPipe(Uri responseAddress)
            {
                _responseAddress = responseAddress;
            }

            public Guid RequestId
            {
                get { return _requestId; }
            }

            public async Task Send(SendContext<TRequest> context)
            {
                _requestId = NewId.NextGuid();

                context.RequestId = _requestId;
                context.ResponseAddress = _responseAddress;
            }

            public bool Visit(IPipelineVisitor visitor)
            {
                return visitor.Visit(this);
            }
        }


        class TimeoutExpired :
            RequestTimeoutExpired
        {
            readonly Guid _correlationId;
            readonly DateTime _expirationTime;
            readonly Guid _requestId;
            readonly DateTime _timestamp;

            public TimeoutExpired(DateTime timestamp, DateTime expirationTime, Guid correlationId, Guid requestId)
            {
                _timestamp = timestamp;
                _expirationTime = expirationTime;
                _correlationId = correlationId;
                _requestId = requestId;
            }

            public DateTime Timestamp
            {
                get { return _timestamp; }
            }

            public DateTime ExpirationTime
            {
                get { return _expirationTime; }
            }

            public Guid CorrelationId
            {
                get { return _correlationId; }
            }

            public Guid RequestId
            {
                get { return _requestId; }
            }
        }
    }
}