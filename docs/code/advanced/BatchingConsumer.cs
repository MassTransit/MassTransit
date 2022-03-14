namespace BatchingConsumer
{
    using System.Threading.Tasks;
    using MassTransit;

    public interface OrderAudit
    {
    }

    class OrderAuditConsumer :
        IConsumer<Batch<OrderAudit>>
    {
        public async Task Consume(ConsumeContext<Batch<OrderAudit>> context)
        {
            for(int i = 0; i < context.Message.Length; i++)
            {
                ConsumeContext<OrderAudit> audit = context.Message[i];
            }
        }
    }

    class OrderAuditConsumerDefinition :
        ConsumerDefinition<OrderAuditConsumer>
    {
        public OrderAuditConsumerDefinition()
        {
            Endpoint(x => x.PrefetchCount = 1000);
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<OrderAuditConsumer> consumerConfigurator)
        {
            consumerConfigurator.Options<BatchOptions>(options => options
                .SetMessageLimit(100)
                .SetTimeLimit(1000)
                .SetConcurrencyLimit(10));
        }
    }
}
