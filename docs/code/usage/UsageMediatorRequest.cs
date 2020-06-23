namespace UsageMediatorRequest
{
    using System;
    using System.Threading.Tasks;
    using UsageContracts;
    using UsageConsumer;
    using UsageMediatorConsumer;
    using MassTransit;
    using MassTransit.Mediator;

    public class Program
    {
        public static async Task Main()
        {
            IMediator mediator = Bus.Factory.CreateMediator(cfg =>
            {
                cfg.Consumer<SubmitOrderConsumer>();
                cfg.Consumer<OrderStatusConsumer>();
            });

            Guid orderId = NewId.NextGuid();

            await mediator.Send<SubmitOrder>(new { OrderId = orderId });

            var client = mediator.CreateRequestClient<GetOrderStatus>();

            var response = await client.GetResponse<OrderStatus>(new { OrderId = orderId });

            Console.WriteLine("Order Status: {0}", response.Message.Status);
        }
    }
}