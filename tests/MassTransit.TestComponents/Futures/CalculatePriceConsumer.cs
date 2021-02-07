namespace MassTransit.TestComponents.Futures
{
    using System.Threading.Tasks;
    using Registration;
    using TestFramework;


    public class CalculatePriceConsumer :
        IConsumer<CalculatePrice>
    {
        public Task Consume(ConsumeContext<CalculatePrice> context)
        {
            if (context.Message.Sku == "missing")
                throw new IntentionalTestException("The sku was invalid");

            return context.RespondAsync<PriceCalculation>(new
            {
                context.Message.OrderLineId,
                Amount = 1234.55m,
            });
        }
    }


    public class CalculatePriceConsumerDefinition :
        FutureRequestConsumerDefinition<CalculatePriceConsumer, CalculatePrice>
    {
    }
}
