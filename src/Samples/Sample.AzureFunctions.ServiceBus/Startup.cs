using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Sample.AzureFunctions.ServiceBus.Startup))]


namespace Sample.AzureFunctions.ServiceBus
{
    using MassTransit;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Consumers;


    public class Startup :
        FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddScoped<SubmitOrderFunctions>()
                .AddScoped<AuditOrderFunctions>()
                .AddMassTransitForAzureFunctions(cfg =>
                {
                    cfg.AddConsumersFromNamespaceContaining<ConsumerNamespace>();
                })
                .AddMassTransitEventHub();
        }
    }
}
