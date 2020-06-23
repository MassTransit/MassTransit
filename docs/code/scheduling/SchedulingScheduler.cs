namespace SchedulingScheduler
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;

    public class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            Uri schedulerEndpoint = new Uri("queue:scheduler");

            services.AddMassTransit(x =>
            {
                x.AddMessageScheduler(schedulerEndpoint);

                x.UsingRabbitMq((context, cfg) => 
                {
                    cfg.UseMessageScheduler(schedulerEndpoint);

                    cfg.ConfigureEndpoints(context);
                });
            });

            var provider = services.BuildServiceProvider();

            var scheduler = provider.GetRequiredService<IMessageScheduler>();

            await scheduler.SchedulePublish<SendNotification>(
                DateTime.UtcNow + TimeSpan.FromSeconds(30), new 
                {
                    EmailAddress = "frank@nul.org",
                    Body =  "Thank you for signing up for our awesome newsletter!"
                });
        }
    }

    public interface SendNotification
    {
        string EmailAddress { get; }
        string Body { get; }
    }
}