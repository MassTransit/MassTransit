namespace MassTransit.TestFramework.Futures
{
    using DependencyInjection.Registration;


    public class PriceCalculationFuture :
        RequestConsumerFuture<CalculatePrice, PriceCalculation>
    {
        public PriceCalculationFuture(IFutureDefinition<PriceCalculationFuture> definition)
            : base(definition)
        {
            ConfigureCommand(x => x.CorrelateById(context => context.Message.OrderLineId));
        }
    }


    public class PriceCalculationFutureDefinition :
        RequestConsumerFutureDefinition<PriceCalculationFuture, CalculatePriceConsumer, CalculatePrice, PriceCalculation>
    {
        public PriceCalculationFutureDefinition(IConsumerDefinition<CalculatePriceConsumer> consumerDefinition)
            : base(consumerDefinition)
        {
        }
    }
}
