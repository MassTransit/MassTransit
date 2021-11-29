namespace MassTransit.Courier
{
    using System.Threading.Tasks;
    using Contracts;


    public abstract class RoutingSlipRequestProxy<TRequest> :
        IConsumer<TRequest>
        where TRequest : class
    {
        public virtual async Task Consume(ConsumeContext<TRequest> context)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddSubscription(context.ReceiveContext.InputAddress, RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            builder.AddVariable("RequestId", context.RequestId);
            builder.AddVariable("ResponseAddress", context.ResponseAddress);
            builder.AddVariable("FaultAddress", context.FaultAddress);
            builder.AddVariable("Request", context.Message);

            await BuildRoutingSlip(builder, context);

            var routingSlip = builder.Build();

            await context.Execute(routingSlip).ConfigureAwait(false);
        }

        protected abstract Task BuildRoutingSlip(RoutingSlipBuilder builder, ConsumeContext<TRequest> request);
    }
}
