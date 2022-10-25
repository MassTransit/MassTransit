namespace UsageMediatorConsumer;

using System;
using System.Threading.Tasks;
using MassTransit;

public record GetOrderStatus
{
    public Guid OrderId { get; init; }
}

public record OrderStatus
{
    public Guid OrderId { get; init; }
    public string Status { get; init; }
}

class OrderStatusConsumer :
    IConsumer<GetOrderStatus>
{
    public async Task Consume(ConsumeContext<GetOrderStatus> context)
    {
        await context.RespondAsync<OrderStatus>(new
        {
            context.Message.OrderId,
            Status = "Pending"
        });
    }
}
