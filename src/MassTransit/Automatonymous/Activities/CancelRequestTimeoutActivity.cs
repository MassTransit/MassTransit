namespace Automatonymous.Activities
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Util;


    public class CancelRequestTimeoutActivity<TInstance, TData, TRequest, TResponse> :
        Activity<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TRequest : class
        where TResponse : class
    {
        readonly Request<TInstance, TRequest, TResponse> _request;

        public CancelRequestTimeoutActivity(Request<TInstance, TRequest, TResponse> request)
        {
            _request = request;
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        async Task Activity<TInstance, TData>.Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            await Execute(context).ConfigureAwait(false);

            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("cancelRequest");
        }

        Task Execute(BehaviorContext<TInstance> context)
        {
            ConsumeContext consumeContext = context.CreateConsumeContext();

            Guid? requestId = _request.GetRequestId(context.Instance);
            if (requestId.HasValue && _request.Settings.Timeout > TimeSpan.Zero)
            {
                if (consumeContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                    return schedulerContext.CancelScheduledSend(consumeContext.ReceiveContext.InputAddress, requestId.Value);

                throw new ConfigurationException("A scheduler was not available to cancel the scheduled request timeout");
            }

            return TaskUtil.Completed;
        }
    }
}
