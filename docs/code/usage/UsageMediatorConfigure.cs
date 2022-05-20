namespace UsageMediatorConfigure
{
    using System;
    using System.Threading.Tasks;
    using UsageContracts;
    using UsageConsumer;
    using UsageMediatorConsumer;
    using MassTransit;
    using MassTransit.Mediator;
    using Microsoft.Extensions.DependencyInjection;

    public class ValidateOrderStatusFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        public void Probe(ProbeContext context)
        {
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (context.Message is GetOrderStatus getOrderStatus && getOrderStatus.OrderId == Guid.Empty)
                throw new ArgumentException("The OrderId must not be empty");

            return next.Send(context);
        }
    }

    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMediator(cfg =>
            {
                cfg.AddConsumer<SubmitOrderConsumer>();
                cfg.AddConsumer<OrderStatusConsumer>();

                cfg.ConfigureMediator((context, mcfg) =>
                {
                    mcfg.UseSendFilter(typeof(ValidateOrderStatusFilter<>), context);
                });
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
