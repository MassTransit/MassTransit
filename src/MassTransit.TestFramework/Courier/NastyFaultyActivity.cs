namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class NastyFaultyActivity :
        IActivity<FaultyArguments, FaultyLog>
    {
        public Task<ExecutionResult> Execute(ExecuteContext<FaultyArguments> context)
        {
            Console.WriteLine("NastyFaultyActivity: Execute");
            Console.WriteLine("NastyFaultyActivity: About to blow this up!");

            throw new InvalidOperationException("Things that make you go boom!");
        }

        public async Task<CompensationResult> Compensate(CompensateContext<FaultyLog> context)
        {
            return context.Compensated();
        }
    }
}
