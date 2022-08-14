namespace MassTransitBenchmark.BusOutbox;

using System;
using MassTransit;


public class RabbitMqConfigureBusOutboxTransport :
    IConfigureBusOutboxTransport
{
    readonly RabbitMqHostSettings _hostSettings;
    readonly BusOutboxBenchmarkOptions _options;

    public RabbitMqConfigureBusOutboxTransport(RabbitMqOptionSet hostSettings, BusOutboxBenchmarkOptions options)
    {
        _hostSettings = hostSettings;
        _options = options;
    }

    public void Using(IBusRegistrationConfigurator configurator, Action<IBusRegistrationContext, IBusFactoryConfigurator> callback)
    {
        configurator.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(_hostSettings);

            callback(context, cfg);

            cfg.ConfigureEndpoints(context);
        });
    }
}
