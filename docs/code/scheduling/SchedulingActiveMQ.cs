namespace SchedulingActiveMQ
{
    using System;
    using MassTransit;
    using MassTransit.ActiveMqTransport;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddActiveMqMessageScheduler();

                x.UsingActiveMq((context, cfg) => 
                {
                    cfg.UseActiveMqMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}