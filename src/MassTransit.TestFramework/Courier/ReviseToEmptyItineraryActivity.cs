namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;


    public class ReviseToEmptyItineraryActivity :
        IActivity<TestArguments, TestLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<TestArguments> context)
        {
            Console.WriteLine("ReviseToEmptyItineraryActivity: Execute: {0}", context.Arguments.Value);

            return context.ReviseItinerary(x =>
            {
            });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            return context.Compensated();
        }
    }
}
