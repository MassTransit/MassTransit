namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class ReviseWithNoChangeItineraryActivity :
        IActivity<TestArguments, TestLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<TestArguments> context)
        {
            Console.WriteLine("ReviseWithNoChangeItineraryActivity: Execute: {0}", context.Arguments.Value);

            return context.ReviseItinerary(x => x.AddActivitiesFromSourceItinerary());
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            return context.Compensated();
        }
    }
}
