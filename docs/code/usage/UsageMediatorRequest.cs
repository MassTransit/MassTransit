namespace UsageMediatorRequest
{
    using System;
    using System.Threading.Tasks;
    using UsageContracts;
    using Microsoft.Extensions.DependencyInjection;
    using UsageMediatorConsumer;
    using MassTransit;
    using MassTransit.Mediator;

    public class Program
    {
        public static async Task Main()
        {
            await using var provider = new ServiceCollection()
                .AddMediator(cfg => { })
                .BuildServiceProvider();

            var mediator = provider.GetRequiredService<IMediator>();

            Guid orderId = NewId.NextGuid();

            await mediator.Send<SubmitOrder>(new { OrderId = orderId });

            var client = mediator.CreateRequestClient<GetOrderStatus>();

            var response = await client.GetResponse<OrderStatus>(new { OrderId = orderId });

            Console.WriteLine("Order Status: {0}", response.Message.Status);
        }
    }
}
