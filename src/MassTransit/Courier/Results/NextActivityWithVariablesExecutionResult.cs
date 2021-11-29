namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class NextActivityWithVariablesExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        readonly IDictionary<string, object> _variables;

        public NextActivityWithVariablesExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, IDictionary<string, object> variables)
            : base(context, publisher, activity, routingSlip)
        {
            _variables = variables;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            builder.SetVariables(_variables);
        }
    }


    class NextActivityWithVariablesExecutionResult<TArguments, TLog> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
        where TLog : class
    {
        readonly Uri _compensationAddress;
        readonly IDictionary<string, object> _variables;

        public NextActivityWithVariablesExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, Uri compensationAddress, IDictionary<string, object> data, IDictionary<string, object> variables)
            : base(context, publisher, activity, routingSlip, data)
        {
            _compensationAddress = compensationAddress;
            _variables = variables;
        }

        public NextActivityWithVariablesExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, Uri compensationAddress, TLog log, IDictionary<string, object> variables)
            : base(context, publisher, activity, routingSlip, RoutingSlipBuilder.GetObjectAsDictionary(log))
        {
            _compensationAddress = compensationAddress;
            _variables = variables;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            builder.AddCompensateLog(Context.ExecutionId, _compensationAddress, Data);
            builder.SetVariables(_variables);
        }
    }
}
