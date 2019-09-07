namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class TestActivity :
        IActivity<TestArguments, TestLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<TestArguments> context)
        {
            if (context.Arguments.Value == null)
                throw new ArgumentNullException(nameof(context.Arguments.Value));

            return context.CompletedWithVariables<TestLog>(new {OriginalValue = context.Arguments.Value}, new
            {
                Value = "Hello, World!",
                NullValue = (string)null,
            });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            Console.WriteLine("TestActivity: Compensate original value: {0}", context.Log.OriginalValue);

            return context.Compensated();
        }
    }
}
