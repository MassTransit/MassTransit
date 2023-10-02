namespace MassTransit.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Events;


    public abstract class RoutingSlipResponseProxy<TRequest, TResponse, TFault> :
        IConsumer<RoutingSlipCompleted>,
        IConsumer<RoutingSlipFaulted>
        where TRequest : class
        where TResponse : class
        where TFault : class
    {
        protected virtual IRetryPolicy RetryPolicy => null;

        public virtual async Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            var requestInfo = new RoutingSlipRequestInfo<TRequest>(context.SerializerContext, context.Message.Variables);

            var endpoint = await context.GetResponseEndpoint<TResponse>(requestInfo.ResponseAddress, requestInfo.RequestId).ConfigureAwait(false);

            var response = await CreateResponseMessage(context, requestInfo.Request).ConfigureAwait(false);

            await endpoint.Send(response).ConfigureAwait(false);
        }

        public virtual async Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            var requestInfo = new RoutingSlipRequestInfo<TRequest>(context.SerializerContext, context.Message.Variables);

            if (CanRetry(requestInfo, context, out TimeSpan? delay))
            {
                var retryAttempt = requestInfo.RetryAttempt ?? 0;

                var schedulerContext = context.GetPayload<MessageSchedulerContext>();

                await schedulerContext.ScheduleSend(requestInfo.RequestAddress, delay.Value, requestInfo.Request, x =>
                {
                    x.RequestId = requestInfo.RequestId;
                    x.ResponseAddress = requestInfo.ResponseAddress;
                    x.FaultAddress = requestInfo.FaultAddress;
                    x.Delay = delay;
                    x.Headers.Set(MessageHeaders.Request.RoutingSlipRetryCount, retryAttempt + 1);
                }).ConfigureAwait(false);

                return;
            }

            var endpoint = await context.GetFaultEndpoint<TRequest>(requestInfo.FaultAddress ?? requestInfo.ResponseAddress, requestInfo.RequestId)
                .ConfigureAwait(false);

            var response = await CreateFaultedResponseMessage(context, requestInfo.Request, requestInfo.RequestId);

            await endpoint.Send(response, x =>
            {
                if (requestInfo.RetryAttempt > 0)
                    x.Headers.Set(MessageHeaders.FaultRetryCount, requestInfo.RetryAttempt.Value);
            }).ConfigureAwait(false);
        }

        bool CanRetry(RoutingSlipRequestInfo<TRequest> requestInfo, ConsumeContext<RoutingSlipFaulted> context, out TimeSpan? delay)
        {
            delay = default;

            var retryPolicy = RetryPolicy;
            if (retryPolicy == null)
                return false;

            RetryPolicyContext<ConsumeContext<RoutingSlipFaulted>> policyContext = retryPolicy.CreatePolicyContext(context);

            var exception = new RoutingSlipRequestFaultedException(context.Message);

            if (!policyContext.CanRetry(exception, out RetryContext<ConsumeContext<RoutingSlipFaulted>> retryContext))
                return false;

            var retryAttempt = requestInfo.RetryAttempt ?? 0;
            for (var retryIndex = 0; retryIndex < retryAttempt; retryIndex++)
            {
                if (!retryContext.CanRetry(exception, out retryContext))
                    return false;
            }

            delay = retryContext.Delay ?? TimeSpan.Zero;
            return true;
        }

        protected abstract Task<TResponse> CreateResponseMessage(ConsumeContext<RoutingSlipCompleted> context, TRequest request);

        protected abstract Task<TFault> CreateFaultedResponseMessage(ConsumeContext<RoutingSlipFaulted> context, TRequest request, Guid requestId);
    }


    public abstract class RoutingSlipResponseProxy<TRequest, TResponse> :
        RoutingSlipResponseProxy<TRequest, TResponse, Fault<TRequest>>
        where TRequest : class
        where TResponse : class
    {
        protected override Task<Fault<TRequest>> CreateFaultedResponseMessage(ConsumeContext<RoutingSlipFaulted> context, TRequest request, Guid requestId)
        {
            IEnumerable<ExceptionInfo> exceptions = context.Message.ActivityExceptions.Select(x => x.ExceptionInfo);

            Fault<TRequest> response = new FaultEvent<TRequest>(request, requestId, context.Host, exceptions, MessageTypeCache<TRequest>.MessageTypeNames);

            return Task.FromResult(response);
        }
    }
}
