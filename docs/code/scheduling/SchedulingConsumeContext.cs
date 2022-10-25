namespace SchedulingConsumeContext;

using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main()
    {
        var services = new ServiceCollection();

        Uri schedulerEndpoint = new Uri("queue:scheduler");

        services.AddMassTransit(x =>
        {
            x.AddMessageScheduler(schedulerEndpoint);

            x.AddConsumer<ScheduleNotificationConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseMessageScheduler(schedulerEndpoint);

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}

public class ScheduleNotificationConsumer :
    IConsumer<ScheduleNotification>
{
    public async Task Consume(ConsumeContext<ScheduleNotification> context)
    {
        Uri notificationService = new Uri("queue:notification-service");

        await context.ScheduleSend<SendNotification>(notificationService,
            context.Message.DeliveryTime,
            new()
            {
                EmailAddress = context.Message.EmailAddress,
                Body = context.Message.Body
            });
    }
}

public record ScheduleNotification
{
    public DateTime DeliveryTime { get; init; }
    public string EmailAddress { get; init; }
    public string Body { get; init; }
}

public record SendNotification
{
    public string EmailAddress { get; init; }
    public string Body { get; init; }
}
