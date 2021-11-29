namespace MassTransit.Courier.Results
{
    using System.Collections.Generic;
    using Contracts;


    class CompensatedWithVariablesCompensationResult<TLog> :
        CompensatedCompensationResult<TLog>
        where TLog : class
    {
        readonly IDictionary<string, object> _variables;

        public CompensatedWithVariablesCompensationResult(CompensateContext<TLog> compensateContext, IRoutingSlipEventPublisher publisher,
            CompensateLog compensateLog,
            RoutingSlip routingSlip,
            IDictionary<string, object> variables)
            : base(compensateContext, publisher, compensateLog, routingSlip)
        {
            _variables = variables;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            builder.SetVariables(_variables);
        }
    }
}
