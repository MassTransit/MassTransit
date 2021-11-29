namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Contracts;
    using Internals;


    public class StateMachineRequest<TInstance, TRequest, TResponse> :
        Request<TInstance, TRequest, TResponse>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        readonly IList<string> _accept;
        readonly IReadProperty<TInstance, Guid?> _read;
        readonly IWriteProperty<TInstance, Guid?> _write;

        public StateMachineRequest(string name, RequestSettings settings, Expression<Func<TInstance, Guid?>> requestIdExpression = default)
        {
            Name = name;
            Settings = settings;

            _accept = new List<string>();

            AcceptResponse<TResponse>();

            if (requestIdExpression != null)
            {
                var propertyInfo = requestIdExpression.GetPropertyInfo();

                _read = ReadPropertyCache<TInstance>.GetProperty<Guid?>(propertyInfo);
                _write = WritePropertyCache<TInstance>.GetProperty<Guid?>(propertyInfo);
            }
        }

        public string Name { get; }
        public RequestSettings Settings { get; }
        public Event<TResponse> Completed { get; set; }
        public Event<Fault<TRequest>> Faulted { get; set; }
        public Event<RequestTimeoutExpired<TRequest>> TimeoutExpired { get; set; }
        public State Pending { get; set; }

        public void SetRequestId(TInstance instance, Guid? requestId)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _write?.Set(instance, requestId);
        }

        public Guid? GetRequestId(TInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return _read != null
                ? _read.Get(instance)
                : instance.CorrelationId;
        }

        public Guid GenerateRequestId(TInstance instance)
        {
            return _read != null
                ? NewId.NextGuid()
                : instance.CorrelationId;
        }

        public void SetSendContextHeaders(SendContext<TRequest> context)
        {
            context.Headers.Set(MessageHeaders.Request.Accept, _accept);
        }

        public bool EventFilter(BehaviorContext<TInstance, RequestTimeoutExpired<TRequest>> context)
        {
            if (!context.TryGetPayload(out ConsumeContext<RequestTimeoutExpired<TRequest>> consumeContext))
                return false;

            if (!consumeContext.RequestId.HasValue)
                return false;

            Guid? requestId = GetRequestId(context.Saga);

            return requestId.HasValue && requestId.Value == consumeContext.RequestId.Value;
        }

        protected void AcceptResponse<T>()
            where T : class
        {
            _accept.Add(MessageUrn.ForTypeString<T>());
        }
    }


    public class StateMachineRequest<TInstance, TRequest, TResponse, TResponse2> :
        StateMachineRequest<TInstance, TRequest, TResponse>,
        Request<TInstance, TRequest, TResponse, TResponse2>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TResponse2 : class
    {
        public StateMachineRequest(string name, RequestSettings settings, Expression<Func<TInstance, Guid?>> requestIdExpression = default)
            : base(name, settings, requestIdExpression)
        {
            AcceptResponse<TResponse2>();
        }

        public Event<TResponse2> Completed2 { get; set; }
    }


    public class StateMachineRequest<TInstance, TRequest, TResponse, TResponse2, TResponse3> :
        StateMachineRequest<TInstance, TRequest, TResponse, TResponse2>,
        Request<TInstance, TRequest, TResponse, TResponse2, TResponse3>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TResponse2 : class
        where TResponse3 : class
    {
        public StateMachineRequest(string name, RequestSettings settings, Expression<Func<TInstance, Guid?>> requestIdExpression = default)
            : base(name, settings, requestIdExpression)
        {
            AcceptResponse<TResponse3>();
        }

        public Event<TResponse3> Completed3 { get; set; }
    }
}
