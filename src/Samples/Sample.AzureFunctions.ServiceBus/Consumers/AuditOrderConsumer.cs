namespace Sample.AzureFunctions.ServiceBus.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Context;


    public class AuditOrderConsumer :
        IConsumer<OrderReceived>
    {
        public async Task Consume(ConsumeContext<OrderReceived> context)
        {
            LogContext.Debug?.Log("Received Order: {OrderNumber}", context.Message.OrderNumber);
        }
    }
}
