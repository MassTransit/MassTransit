namespace MassTransit.Abstractions.Tests.Usage
{
    using System;
    using System.Threading.Tasks;
    using Courier;


    public class ProcessOrderActivity :
        IActivity<ProcessOrderArguments, ProcessOrderLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<ProcessOrderArguments> context)
        {
            Guid shipmentId = NewId.NextGuid();

            return context.Completed<ProcessOrderLog>(new { shipmentId });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<ProcessOrderLog> context)
        {
            return context.Compensated();
        }
    }


    public class ProcessOrderActivityDefinition :
        ActivityDefinition<ProcessOrderActivity, ProcessOrderArguments, ProcessOrderLog>
    {
        public ProcessOrderActivityDefinition()
        {
            ExecuteEndpoint(x => x.PrefetchCount = 16);
            CompensateEndpoint(x => x.PrefetchCount = 4);
        }

        protected override void ConfigureExecuteActivity(IReceiveEndpointConfigurator endpointConfigurator,
            IExecuteActivityConfigurator<ProcessOrderActivity, ProcessOrderArguments> executeActivityConfigurator)
        {
        }

        protected override void ConfigureCompensateActivity(IReceiveEndpointConfigurator endpointConfigurator,
            ICompensateActivityConfigurator<ProcessOrderActivity, ProcessOrderLog> compensateActivityConfigurator)
        {
        }
    }
}
