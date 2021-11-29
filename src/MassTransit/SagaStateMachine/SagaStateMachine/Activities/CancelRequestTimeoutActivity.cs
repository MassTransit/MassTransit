namespace MassTransit.SagaStateMachine
{
    using System;
    using System.Threading.Tasks;


    public class CancelRequestTimeoutActivity<TSaga, TMessage, TRequest, TResponse> :
        IStateMachineActivity<TSaga, TMessage>
        where TSaga : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
        where TMessage : class
    {
        readonly Request<TSaga, TRequest, TResponse> _request;

        public CancelRequestTimeoutActivity(Request<TSaga, TRequest, TResponse> request)
        {
            _request = request;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("cancelRequest");
        }

        public async Task Execute(BehaviorContext<TSaga, TMessage> context, IBehavior<TSaga, TMessage> next)
        {
            Guid? requestId = _request.GetRequestId(context.Saga);
            if (requestId.HasValue && _request.Settings.Timeout > TimeSpan.Zero)
            {
                if (context.TryGetPayload(out MessageSchedulerContext schedulerContext))
                    await schedulerContext.CancelScheduledSend(context.ReceiveContext.InputAddress, requestId.Value).ConfigureAwait(false);
                else
                    throw new ConfigurationException("A scheduler was not available to cancel the scheduled request timeout");
            }

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TSaga, TMessage, TException> context, IBehavior<TSaga, TMessage> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}
