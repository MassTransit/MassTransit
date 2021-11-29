namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;


    class ReviseItineraryExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        readonly Action<IItineraryBuilder> _itineraryBuilder;

        public ReviseItineraryExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip,
            Action<IItineraryBuilder> itineraryBuilder)
            : base(context, publisher, activity, routingSlip)
        {
            _itineraryBuilder = itineraryBuilder;
        }

        public ReviseItineraryExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip,
            IDictionary<string, object> data, Action<IItineraryBuilder> itineraryBuilder)
            : base(context, publisher, activity, routingSlip, data)
        {
            _itineraryBuilder = itineraryBuilder;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            _itineraryBuilder(builder);
        }

        protected override RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, Enumerable.Empty<Activity>(), routingSlip.Itinerary.Skip(1));
        }

        protected override async Task PublishActivityEvents(RoutingSlip routingSlip, RoutingSlipBuilder builder)
        {
            await base.PublishActivityEvents(routingSlip, builder).ConfigureAwait(false);

            await Publisher.PublishRoutingSlipRevised(Context.ActivityName, Context.ExecutionId, Context.Timestamp, Context.Elapsed, routingSlip.Variables,
                routingSlip.Itinerary, builder.SourceItinerary).ConfigureAwait(false);
        }
    }


    class ReviseItineraryExecutionResult<TArguments, TLog> :
        ReviseItineraryExecutionResult<TArguments>
        where TArguments : class
    {
        readonly Uri _compensationAddress;

        public ReviseItineraryExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip,
            Uri compensationAddress, TLog log, Action<IItineraryBuilder> itineraryBuilder)
            : base(context, publisher, activity, routingSlip, RoutingSlipBuilder.GetObjectAsDictionary(log), itineraryBuilder)
        {
            _compensationAddress = compensationAddress;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            builder.AddCompensateLog(Context.ExecutionId, _compensationAddress, Data);

            base.Build(builder);
        }
    }
}
