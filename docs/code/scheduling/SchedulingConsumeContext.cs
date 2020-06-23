namespace SchedulingConsumeContext
{
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
                new 
                {
                    EmailAddress = context.Message.EmailAddress,
                    Body =  context.Message.Body
                });
        }

        class SendNotificationCommand :
            SendNotification
        {
            public string EmailAddress { get; set; }
            public string Body { get; set; }
        }
    }

    public interface ScheduleNotification
    {
        DateTime DeliveryTime { get; }
        string EmailAddress { get; }
        string Body { get; }
    }

    public interface SendNotification
    {
        string EmailAddress { get; }
        string Body { get; }
    }
}