namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;
    using Contracts;


    /// <summary>
    /// Publishes the <see cref="RequestCompleted" /> event, used by the request state machine to track
    /// pending requests for a saga instance.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    public class RequestFaultedActivity<TSaga, TMessage, TRequest> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TMessage : class
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

        public async Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            var payload = context.Message as Fault;

            await context.Publish<RequestFaulted>(new
            {
                context.Saga.CorrelationId,
                PayloadType = MessageTypeCache<Fault<TRequest>>.MessageTypeNames,
                Payload = new
                {
                    payload.FaultId,
                    payload.FaultedMessageId,
                    payload.Timestamp,
                    payload.Host,
                    payload.Exceptions
                }
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
