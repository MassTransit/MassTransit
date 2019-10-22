namespace Automatonymous.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Context;


    public class MissingInstanceRedeliveryPipe<TInstance, TData> :
        IPipe<ConsumeContext<TData>>
        where TInstance : SagaStateMachineInstance
        where TData : class
    {
        readonly IRetryPolicy _retryPolicy;
        readonly IPipe<ConsumeContext<TData>> _finalPipe;

        public MissingInstanceRedeliveryPipe(IRetryPolicy retryPolicy, IPipe<ConsumeContext<TData>> finalPipe)
        {
            _retryPolicy = retryPolicy;
            _finalPipe = finalPipe;
        }

        public Task Send(ConsumeContext<TData> context)
        {
            using (RetryPolicyContext<ConsumeContext<TData>> policyContext = _retryPolicy.CreatePolicyContext(context))
            {
                var exception = new SagaException("An existing saga instance was not found", typeof(TInstance), typeof(TData), context.CorrelationId ?? Guid
                    .Empty);

                if (!policyContext.CanRetry(exception, out RetryContext<ConsumeContext<TData>> retryContext))
                {
                    return _finalPipe.Send(context);
                }

                int previousDeliveryCount = context.GetRedeliveryCount();
                for (int retryIndex = 0; retryIndex < previousDeliveryCount; retryIndex++)
                {
                    if (!retryContext.CanRetry(exception, out retryContext))
                    {
                        return _finalPipe.Send(context);
                    }
                }

                MessageRedeliveryContext redeliveryContext = new ScheduleMessageRedeliveryContext<TData>(context);

                var delay = retryContext.Delay ?? TimeSpan.Zero;

                return redeliveryContext.ScheduleRedelivery(delay);
            }
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
