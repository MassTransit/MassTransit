namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes;
    using MassTransit.Contracts;
    using MassTransit.Initializers;
    using MassTransit.Metadata;
    using MassTransit.Util;


    /// <summary>
    /// Publishes the <see cref="Automatonymous.Contracts.RequestStarted"/> event, used by the request state machine to track
    /// pending requests for a saga instance.
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class RequestStartedActivity<TInstance, TData> :
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

            var initializeContext = await MessageInitializerCache<RequestStarted<TData>>.Initialize(new
            {
                context.Instance.CorrelationId,
                consumeContext.RequestId,
                consumeContext.ResponseAddress,
                consumeContext.FaultAddress,
                consumeContext.ExpirationTime,
                PayloadType = TypeMetadataCache<TData>.MessageTypeNames,
                Payload = context.Data
            }).ConfigureAwait(false);

            object message = initializeContext.Message;

            await consumeContext.Publish(message, typeof(RequestStarted)).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
