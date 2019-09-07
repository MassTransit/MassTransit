namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class FaultyActivity :
        IActivity<FaultyArguments, FaultyLog>,
        IDisposable
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<FaultyArguments> context)
        {
            Console.WriteLine("FaultyActivity: Execute");
            Console.WriteLine("FaultyActivity: About to blow this up!");

            throw new IntentionalTestException("Things that make you go boom!");
        }

        public async Task<CompensationResult> Compensate(CompensateContext<FaultyLog> context)
        {
            return context.Compensated();
        }

        public void Dispose()
        {
        }
    }
}
