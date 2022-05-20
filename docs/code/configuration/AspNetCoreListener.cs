namespace AspNetCoreListener
{
    using System;
    using System.Threading.Tasks;
    using EventContracts;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<EventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        e.ConfigureConsumer<EventConsumer>(context);
                    });
                });
            });
        }
    }

    class EventConsumer :
        IConsumer<ValueEntered>
    {
        ILogger<EventConsumer> _logger;

        public EventConsumer(ILogger<EventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ValueEntered> context)
        {
            _logger.LogInformation("Value: {Value}", context.Message.Value);
        }
    }
}
