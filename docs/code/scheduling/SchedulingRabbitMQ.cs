namespace SchedulingRabbitMQ
{
    using System;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddRabbitMqMessageScheduler();

                x.UsingRabbitMq((context, cfg) => 
                {
                    cfg.UseDelayedExchangeMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}