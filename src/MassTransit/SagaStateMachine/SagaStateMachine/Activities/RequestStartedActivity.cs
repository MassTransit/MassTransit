namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;
    using Contracts;


    /// <summary>
    /// Publishes the <see cref="RequestStarted" /> event, used by the request state machine to track
    /// pending requests for a saga instance.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class RequestStartedActivity<TSaga, TMessage> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("requestStarted");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            await context.Publish<RequestStarted>(new
            {
                context.Saga.CorrelationId,
                context.RequestId,
                context.ResponseAddress,
                context.FaultAddress,
                context.ExpirationTime,
                PayloadType = MessageTypeCache<TMessage>.MessageTypeNames,
                Payload = context.Message
            }).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
