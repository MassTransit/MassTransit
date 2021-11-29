namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class FaultedWithVariablesExecutionResult<TArguments> :
        FaultedExecutionResult<TArguments>
        where TArguments : class
    {
        readonly IDictionary<string, object> _variables;

        public FaultedWithVariablesExecutionResult(ExecuteContext<TArguments> executeContext, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, Exception exception, IDictionary<string, object> variables)
            : base(executeContext, publisher, activity, routingSlip, exception)
        {
            _variables = variables;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            builder.SetVariables(_variables);
        }
    }
}
