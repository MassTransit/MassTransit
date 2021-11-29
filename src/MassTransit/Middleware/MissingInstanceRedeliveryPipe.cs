namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Context;


    public class MissingInstanceRedeliveryPipe<TSaga, TMessage> :
        IPipe<ConsumeContext<TMessage>>
        where TSaga : SagaStateMachineInstance
        where TMessage : class
    {
        readonly IPipe<ConsumeContext<TMessage>> _finalPipe;
        readonly RedeliveryOptions _options;
        readonly IRetryPolicy _retryPolicy;

        public MissingInstanceRedeliveryPipe(IRetryPolicy retryPolicy, IPipe<ConsumeContext<TMessage>> finalPipe, RedeliveryOptions options)
        {
            _retryPolicy = retryPolicy;
            _finalPipe = finalPipe;
            _options = options;
        }

        public Task Send(ConsumeContext<TMessage> context)
        {
            using RetryPolicyContext<ConsumeContext<TMessage>> policyContext = _retryPolicy.CreatePolicyContext(context);

            var exception = new SagaException("An existing saga instance was not found", typeof(TSaga), typeof(TMessage), context.CorrelationId ?? Guid.Empty);

            if (!policyContext.CanRetry(exception, out RetryContext<ConsumeContext<TMessage>> retryContext))
                return _finalPipe.Send(context);

            var previousDeliveryCount = context.GetRedeliveryCount();
            for (var retryIndex = 0; retryIndex < previousDeliveryCount; retryIndex++)
            {
                if (!retryContext.CanRetry(exception, out retryContext))
                    return _finalPipe.Send(context);
            }

            MessageRedeliveryContext redeliveryContext = new ScheduleMessageRedeliveryContext<TMessage>(context, _options);

            var delay = retryContext.Delay ?? TimeSpan.Zero;

            return redeliveryContext.ScheduleRedelivery(delay);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
