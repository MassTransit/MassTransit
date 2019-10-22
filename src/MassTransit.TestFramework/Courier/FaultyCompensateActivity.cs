namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class FaultyCompensateActivity :
        IActivity<TestArguments, TestLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<TestArguments> context)
        {
            Console.WriteLine("FaultyCompensateActivity: Execute: {0}", context.Arguments.Value);

            return context.Completed<TestLog>(new {OriginalValue = context.Arguments.Value});
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            Console.WriteLine("FaultyCompensateActivity: Compensate: {0}", context.Log.OriginalValue);

            return context.Failed();
        }
    }
}
