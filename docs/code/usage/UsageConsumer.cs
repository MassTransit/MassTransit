namespace UsageConsumer;

using System.Threading.Tasks;
using UsageContracts;
using MassTransit;

class SubmitOrderConsumer :
    IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        await context.Publish<OrderSubmitted>(new
        {
            context.Message.OrderId
        });
    }
}
