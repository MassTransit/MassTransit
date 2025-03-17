namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;


    public class TestActivity :
        IActivity<TestArguments, TestLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<TestArguments> context)
        {
            if (context.Arguments.Value == null)
                throw new ArgumentNullException(nameof(context.Arguments.Value));

            return context.Completed<TestLog>(new { OriginalValue = context.Arguments.Value }, x =>
            {
                x.SetVariables(new
                {
                    Value = "Hello, World!",
                    NullValue = (string)null
                });

                if (context.TryGetPayload(out MessageSchedulerContext _))
                    x.Delay = TimeSpan.FromSeconds(1);
            });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            Console.WriteLine("TestActivity: Compensate original value: {0}", context.Log.OriginalValue);

            return context.Compensated();
        }
    }
}
