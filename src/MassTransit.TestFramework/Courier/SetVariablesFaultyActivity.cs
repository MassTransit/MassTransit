namespace MassTransit.TestFramework.Courier
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Courier;
    using MassTransit.Courier.Exceptions;


    public class SetVariablesFaultyActivity :
        IExecuteActivity<SetVariablesFaultyArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<SetVariablesFaultyArguments> context)
        {
            return context.FaultedWithVariables(new IntentionalTestException("Things that make you go boom!"),
                new Dictionary<string, object> {{ "Test", "Data" }});
        }
    }
}
