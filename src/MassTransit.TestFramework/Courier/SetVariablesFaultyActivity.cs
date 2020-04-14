namespace MassTransit.TestFramework.Courier
{
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class SetVariablesFaultyActivity :
        IExecuteActivity<SetVariablesFaultyArguments>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<SetVariablesFaultyArguments> context)
        {
            return context.FaultedWithVariables(new IntentionalTestException("Things that make you go boom!"),
                new {Test = "Data"});
        }
    }
}
