namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;


    abstract class CompletedExecutionResult<TArguments> :
        ExecutionResult
        where TArguments : class
    {
        protected CompletedExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip)
            : this(context, publisher, activity, routingSlip, RoutingSlipBuilder.NoArguments)
        {
        }

        protected CompletedExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip,
            IDictionary<string, object> data)
        {
            Context = context;
            Publisher = publisher;
            Activity = activity;
            RoutingSlip = routingSlip;
            Data = data;
            Duration = Context.Elapsed;
        }

        protected RoutingSlip RoutingSlip { get; }

        protected IRoutingSlipEventPublisher Publisher { get; }

        protected IDictionary<string, object> Data { get; }

        protected ExecuteContext<TArguments> Context { get; }

        protected Activity Activity { get; }

        protected TimeSpan Duration { get; }

        public async Task Evaluate()
        {
            var builder = CreateRoutingSlipBuilder(RoutingSlip);

            Build(builder);

            var routingSlip = builder.Build();

            await PublishActivityEvents(routingSlip, builder).ConfigureAwait(false);

            if (HasNextActivity(routingSlip))
            {
                var endpoint = await Context.GetSendEndpoint(routingSlip.GetNextExecuteAddress()).ConfigureAwait(false);

                await Context.Forward(endpoint, routingSlip).ConfigureAwait(false);
            }
            else
            {
                var completedTimestamp = Context.Timestamp + Duration;
                var completedDuration = completedTimestamp - RoutingSlip.CreateTimestamp;

                await Publisher.PublishRoutingSlipCompleted(completedTimestamp, completedDuration, routingSlip.Variables).ConfigureAwait(false);
            }
        }

        public virtual bool IsFaulted(out Exception exception)
        {
            exception = default;
            return false;
        }

        protected virtual Task PublishActivityEvents(RoutingSlip routingSlip, RoutingSlipBuilder builder)
        {
            return Publisher.PublishRoutingSlipActivityCompleted(Context.ActivityName, Context.ExecutionId, Context.Timestamp, Duration,
                routingSlip.Variables, Activity.Arguments, Data);
        }

        static bool HasNextActivity(RoutingSlip routingSlip)
        {
            return routingSlip.Itinerary.Any();
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityLog(Context.Host, Activity.Name, Context.ExecutionId, Context.Timestamp, Duration);
        }

        protected virtual RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, routingSlip.Itinerary.Skip(1), Enumerable.Empty<Activity>());
        }
    }
}
