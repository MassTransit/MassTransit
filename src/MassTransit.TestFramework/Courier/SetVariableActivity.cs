namespace MassTransit.TestFramework.Courier
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class SetVariableActivity :
        IExecuteActivity<SetVariableArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<SetVariableArguments> context)
        {
            if (context.Arguments == null)
                throw new RoutingSlipException("The arguments for execution were null");

            return context.CompletedWithVariables(new Dictionary<string, object> {{context.Arguments.Key, context.Arguments.Value}});
        }
    }
}
