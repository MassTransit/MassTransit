namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class ReviseItineraryWithVariablesExecutionResult<TArguments, TLog> :
        ReviseItineraryExecutionResult<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly IDictionary<string, object> _variables;

        public ReviseItineraryWithVariablesExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, Uri compensationAddress, TLog log, IDictionary<string, object> variables, Action<IItineraryBuilder> itineraryBuilder)
            : base(context, publisher, activity, routingSlip, compensationAddress, log, itineraryBuilder)
        {
            _variables = variables;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            builder.SetVariables(_variables);

            base.Build(builder);
        }
    }
}
