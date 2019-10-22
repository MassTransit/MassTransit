namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class SecondTestActivity :
        IActivity<TestArguments, TestLog>,
        IDisposable
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<TestArguments> context)
        {
            return context.CompletedWithVariables(new
            {
                ToBeRemoved = (string)null
            });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            return context.Compensated();
        }

        public void Dispose()
        {
        }
    }
}
