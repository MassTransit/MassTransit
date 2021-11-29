namespace MassTransit.TestFramework.Courier
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class SetLargeVariableActivity :
        IExecuteActivity<SetLargeVariableArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<SetLargeVariableArguments> context)
        {
            if (context.Arguments == null)
                throw new RoutingSlipException("The arguments for execution were null");

            var value = await context.Arguments.Value.Value;

            return context.CompletedWithVariables(new Dictionary<string, object> {{context.Arguments.Key, value}});
        }
    }
}
