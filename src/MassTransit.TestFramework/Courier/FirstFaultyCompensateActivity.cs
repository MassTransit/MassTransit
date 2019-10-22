namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class FirstFaultyCompensateActivity :
        IActivity<TestArguments, TestLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<TestArguments> context)
        {
            Console.WriteLine("FirstFaultyCompensateActivity: Execute: {0}", context.Arguments.Value);

            return context.Completed<TestLog>(new {OriginalValue = context.Arguments.Value});
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            Console.WriteLine("FirstFaultyCompensateActivity: Compensate: {0}", context.Log.OriginalValue);

            if (context.GetRetryAttempt() > 0 || context.GetRedeliveryCount() > 0)
                return context.Compensated();

            return context.Failed();
        }
    }
}
