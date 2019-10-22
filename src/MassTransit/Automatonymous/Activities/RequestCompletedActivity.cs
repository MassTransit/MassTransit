namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Contracts;
    using MassTransit.Initializers;
    using MassTransit.Metadata;
    using MassTransit.Util;


    /// <summary>
    /// Publishes the <see cref="Automatonymous.Contracts.RequestCompleted{TResponse}"/> event, used by the request state machine to track
    /// pending requests for a saga instance.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class RequestCompletedActivity<TInstance, TData> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("requestStarted");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var initializeContext = await MessageInitializerCache<RequestCompleted<TData>>.Initialize(new
            {
                context.Instance.CorrelationId,
                InVar.Timestamp,
                PayloadType = TypeMetadataCache<TData>.MessageTypeNames,
                Payload = context.Data
            }).ConfigureAwait(false);

            object message = initializeContext.Message;

            await consumeContext.Publish(message, typeof(RequestCompleted)).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }


    /// <summary>
    /// Publishes the <see cref="Automatonymous.Contracts.RequestCompleted{TResponse}"/> event, used by the request state machine to track
    /// pending requests for a saga instance.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class RequestCompletedActivity<TInstance, TData, TResponse> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TResponse : class
    {
        readonly AsyncEventMessageFactory<TInstance, TData, TResponse> _messageFactory;

        public RequestCompletedActivity(AsyncEventMessageFactory<TInstance, TData, TResponse> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("requestStarted");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var initializeContext = await MessageInitializerCache<RequestCompleted<TData>>.Initialize(new
            {
                context.Instance.CorrelationId,
                InVar.Timestamp,
                PayloadType = TypeMetadataCache<TData>.MessageTypeNames,
                Payload = _messageFactory(consumeContext)
            }).ConfigureAwait(false);

            object message = initializeContext.Message;

            await consumeContext.Publish(message, typeof(RequestCompleted)).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
