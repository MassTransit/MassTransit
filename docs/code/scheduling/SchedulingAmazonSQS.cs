namespace SchedulingAmazonSQS
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
                x.AddAmazonSqsMessageScheduler();

                x.UsingAmazonSqs((context, cfg) => 
                {
                    cfg.UseAmazonSqsMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}