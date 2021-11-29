namespace MassTransit.Abstractions.Tests.Usage
{
    using System.Threading.Tasks;


    /// <summary>
    /// Example Consumer (using abstractions)
    /// </summary>
    public class SubmitOrderConsumer :
        IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            await context.Publish<OrderSubmitted>(new
            {
                CorrelationId = context.Message.OrderId,
                context.Message.OrderId,
                InVar.Timestamp
            }, x => x.Headers.Set("RandomHeader", "RandomValue"));

            if (context.IsResponseAccepted<OrderSubmissionAccepted>())
                await context.RespondAsync<OrderSubmissionAccepted>(new { context.Message.OrderId });
        }
    }


    public class SubmitOrderConsumerDefinition :
        ConsumerDefinition<SubmitOrderConsumer>
    {
        public SubmitOrderConsumerDefinition()
        {
            ConcurrentMessageLimit = 8;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
        {
        }
    }
}
