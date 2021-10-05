namespace SchedulingInternalInstance
{
    using System;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Spi;

    public class Program
    {
        public static void Main()
        {
            const string schedulerQueueName = "scheduler";
            var schedulerUri = new Uri($"queue:{schedulerQueueName}");

            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.AddMessageScheduler(schedulerUri);

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseInMemoryScheduler(schedulerCfg =>
                    {
                        schedulerCfg.QueueName = schedulerQueueName;
                        schedulerCfg.SchedulerFactory = context.GetRequiredService<ISchedulerFactory>();
                        schedulerCfg.JobFactory = context.GetRequiredService<IJobFactory>();
                        schedulerCfg.StartScheduler = false;
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
