namespace SchedulingInMemory
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
                x.AddMessageScheduler(new Uri("queue:scheduler"));

                x.UsingRabbitMq((context, cfg) => 
                {
                    cfg.UseInMemoryScheduler("scheduler");

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}