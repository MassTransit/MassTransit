namespace SchedulingInternalInstance;

using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

public class Program
{
    public static void Main()
    {
        const string schedulerQueueName = "scheduler";
        var schedulerUri = new Uri($"queue:{schedulerQueueName}");

        var services = new ServiceCollection();

        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
        });

        services.AddMassTransit(x =>
        {
            x.AddMessageScheduler(schedulerUri);

            x.AddPublishMessageScheduler();

            x.AddQuartzConsumers();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UsePublishMessageScheduler();

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
