namespace MassTransit.TestFramework.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;


    public class AddressActivity :
        IActivity<AddressArguments, AddressLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<AddressArguments> context)
        {
            Console.WriteLine("Address: {0}", context.Arguments.Address);

            return context.Completed<AddressLog>(new {UsedAddress = context.Arguments.Address});
        }

        public async Task<CompensationResult> Compensate(CompensateContext<AddressLog> context)
        {
            return context.Compensated();
        }
    }
}
