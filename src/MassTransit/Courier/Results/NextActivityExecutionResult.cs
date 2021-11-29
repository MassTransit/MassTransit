namespace MassTransit.Courier.Results
{
    using System;
    using System.Collections.Generic;
    using Contracts;


    class NextActivityExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        public NextActivityExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip)
            : base(context, publisher, activity, routingSlip)
        {
        }
    }


    class NextActivityExecutionResult<TArguments, TLog> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
        where TLog : class
    {
        readonly Uri _compensationAddress;

        public NextActivityExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip,
            Uri compensationAddress, TLog log)
            : base(context, publisher, activity, routingSlip, RoutingSlipBuilder.GetObjectAsDictionary(log))
        {
            _compensationAddress = compensationAddress;
        }

        public NextActivityExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip,
            Uri compensationAddress, IDictionary<string, object> data)
            : base(context, publisher, activity, routingSlip, data)
        {
            _compensationAddress = compensationAddress;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            builder.AddCompensateLog(Context.ExecutionId, _compensationAddress, Data);
        }
    }
}
