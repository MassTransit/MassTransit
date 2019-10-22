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
    /// <typeparam name="TRequest"></typeparam>
    public class RequestFaultedActivity<TInstance, TData, TRequest> :
        Activity<TInstance, TData>
        where TInstance : SagaStateMachineInstance
        where TData : class
        where TRequest : class
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("requestFaulted");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            ConsumeEventContext<TInstance, TData> consumeContext = context.CreateConsumeContext();

            var payload = context.Data as Fault;

            var initializeContext = await MessageInitializerCache<RequestFaulted<TRequest>>.Initialize(new
            {
                context.Instance.CorrelationId,
                PayloadType = TypeMetadataCache<Fault<TRequest>>.MessageTypeNames,
                Payload = new
                {
                    payload.FaultId,
                    payload.FaultedMessageId,
                    payload.Timestamp,
                    payload.Host,
                    payload.Exceptions
                }
            }).ConfigureAwait(false);

            object message = initializeContext.Message;

            await consumeContext.Publish(message, typeof(RequestFaulted)).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
