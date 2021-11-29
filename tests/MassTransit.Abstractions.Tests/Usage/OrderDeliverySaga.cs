namespace MassTransit.Abstractions.Tests.Usage
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Example Consumer Saga (using abstractions)
    /// </summary>
    public class OrderDeliverySaga :
        ISaga,
        InitiatedByOrOrchestrates<OrderSubmitted>
    {
        public DateTime SubmitTimestamp { get; set; }

        public async Task Consume(ConsumeContext<OrderSubmitted> context)
        {
            SubmitTimestamp = context.Message.Timestamp;
        }

        public Guid CorrelationId { get; set; }
    }


    public class OrderDeliverySagaDefinition :
        SagaDefinition<OrderDeliverySaga>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderDeliverySaga> sagaConfigurator)
        {
        }
    }
}
