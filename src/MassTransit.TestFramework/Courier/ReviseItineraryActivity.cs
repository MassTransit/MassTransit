namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class ReviseItineraryActivity :
        IActivity<TestArguments, TestLog>
    {
        readonly Action<IItineraryBuilder> _callback;

        public ReviseItineraryActivity(Action<IItineraryBuilder> callback)
        {
            _callback = callback;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<TestArguments> context)
        {
            Console.WriteLine("ReviseToEmptyItineraryActivity: Execute: {0}", context.Arguments.Value);

            return context.ReviseItinerary(_callback);
        }

        public async Task<CompensationResult> Compensate(CompensateContext<TestLog> context)
        {
            return context.Compensated();
        }
    }
}
