namespace MultiBusContainer
{
    using System;
    using System.Threading.Tasks;
    using ContainerContracts;
    using ContainerConsumers;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

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
        }
    }
}
