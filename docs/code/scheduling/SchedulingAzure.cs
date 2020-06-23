namespace SchedulingAzure
{
    using System;
    using MassTransit;
    using MassTransit.Azure.ServiceBus.Core;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddServiceBusMessageScheduler();

                x.UsingAzureServiceBus((context, cfg) => 
                {
                    cfg.UseServiceBusMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}