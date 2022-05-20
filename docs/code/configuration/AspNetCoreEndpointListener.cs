namespace AspNetCoreEndpointListener
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
                x.AddConsumer<ValueEnteredEventConsumer>();

                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }

    class ValueEnteredEventConsumer :
        IConsumer<ValueEntered>
    {
        ILogger<ValueEnteredEventConsumer> _logger;

        public ValueEnteredEventConsumer(ILogger<ValueEnteredEventConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ValueEntered> context)
        {
            _logger.LogInformation("Value: {Value}", context.Message.Value);
        }
    }
}
