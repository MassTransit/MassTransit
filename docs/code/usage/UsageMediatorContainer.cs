namespace UsageMediatorContainer
{
    using System;
    using System.Threading.Tasks;
    using UsageContracts;
    using UsageConsumer;
    using UsageMediatorConsumer;
    using MassTransit;
    using MassTransit.Mediator;
    using Microsoft.Extensions.DependencyInjection;


    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMediator(cfg =>
            {
                cfg.AddConsumer<SubmitOrderConsumer>();
                cfg.AddConsumer<OrderStatusConsumer>();
            });

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<IMediator>();

            Guid orderId = NewId.NextGuid();

            await mediator.Send<SubmitOrder>(new { OrderId = orderId });

            var client = mediator.CreateRequestClient<GetOrderStatus>();

            var response = await client.GetResponse<OrderStatus>(new { OrderId = orderId });

            Console.WriteLine("Order Status: {0}", response.Message.Status);
        }
    }
}