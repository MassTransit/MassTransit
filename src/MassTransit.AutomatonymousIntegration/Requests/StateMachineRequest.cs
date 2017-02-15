// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.Requests
{
    using System;
    using System.Linq.Expressions;
    using Events;
    using GreenPipes.Internals.Reflection;
    using MassTransit;
    using MassTransit.Internals.Extensions;


    public class StateMachineRequest<TInstance, TRequest, TResponse> :
        Request<TInstance, TRequest, TResponse>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        readonly string _name;
        readonly ReadWriteProperty<TInstance, Guid?> _requestIdProperty;
        readonly RequestSettings _settings;

        public StateMachineRequest(string requestName, Expression<Func<TInstance, Guid?>> requestIdExpression, RequestSettings settings)
        {
            _name = requestName;
            _settings = settings;

            _requestIdProperty = new ReadWriteProperty<TInstance, Guid?>(requestIdExpression.GetPropertyInfo());
        }

        string Request<TInstance, TRequest, TResponse>.Name => _name;
        RequestSettings Request<TInstance, TRequest, TResponse>.Settings => _settings;
        public Event<TResponse> Completed { get; set; }
        public Event<Fault<TRequest>> Faulted { get; set; }
        public Event<RequestTimeoutExpired<TRequest>> TimeoutExpired { get; set; }
        public State Pending { get; set; }

        public void SetRequestId(TInstance instance, Guid? requestId)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _requestIdProperty.Set(instance, requestId);
        }

        public Guid? GetRequestId(TInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return _requestIdProperty.Get(instance);
        }

        public bool EventFilter(EventContext<TInstance, RequestTimeoutExpired<TRequest>> context)
        {
            ConsumeContext<RequestTimeoutExpired<TRequest>> consumeContext;
            if (!context.TryGetPayload(out consumeContext))
                return false;

            if (!consumeContext.RequestId.HasValue)
                return false;

            var requestId = _requestIdProperty.Get(context.Instance);

            return requestId.HasValue && requestId.Value == consumeContext.RequestId.Value;
        }
    }
}