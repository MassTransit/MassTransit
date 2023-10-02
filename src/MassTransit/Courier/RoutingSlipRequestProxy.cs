namespace MassTransit.Courier
{
    using System;
    using System.Threading.Tasks;
    using Contracts;


    public abstract class RoutingSlipRequestProxy<TRequest> :
        IConsumer<TRequest>
        where TRequest : class
    {
        public virtual async Task Consume(ConsumeContext<TRequest> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddSubscription(GetResponseEndpointAddress(context), RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            builder.AddVariable(RoutingSlipRequestVariableNames.RequestId, context.RequestId);
            builder.AddVariable(RoutingSlipRequestVariableNames.ResponseAddress, context.ResponseAddress);
            builder.AddVariable(RoutingSlipRequestVariableNames.FaultAddress, context.FaultAddress);
            builder.AddVariable(RoutingSlipRequestVariableNames.Request, context.Message);
            builder.AddVariable(RoutingSlipRequestVariableNames.RequestAddress, context.ReceiveContext.InputAddress);

            var retryAttempt = context.Headers.Get<int>(MessageHeaders.Request.RoutingSlipRetryCount);
            if (retryAttempt > 0)
                builder.AddVariable(RoutingSlipRequestVariableNames.RetryAttempt, retryAttempt);

            await BuildRoutingSlip(builder, context);

            var routingSlip = builder.Build();

            await context.Execute(routingSlip, context.CancellationToken).ConfigureAwait(false);
        }

        protected abstract Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<TRequest> request);

        /// <summary>
        /// By default, returns the input address of the request consumer which assumes the response consumer is on the same receive endpoint.
        /// Override to specify the endpoint address of the response consumer if it is configured on a separate receive endpoint.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Uri GetResponseEndpointAddress(ConsumeContext<TRequest> context)
        {
            return context.ReceiveContext.InputAddress;
        }
    }
}
