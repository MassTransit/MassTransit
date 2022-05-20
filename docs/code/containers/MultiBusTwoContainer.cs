namespace MultiBusTwoContainer
{
    using ContainerContracts;
    using ContainerConsumers;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

    public interface ISecondBus :
        IBus
    {
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<SubmitOrderConsumer>();
                x.AddRequestClient<SubmitOrder>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddMassTransit<ISecondBus>(x =>
            {
                x.AddConsumer<AllocateInventoryConsumer>();
                x.AddRequestClient<AllocateInventory>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("remote-host");

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
