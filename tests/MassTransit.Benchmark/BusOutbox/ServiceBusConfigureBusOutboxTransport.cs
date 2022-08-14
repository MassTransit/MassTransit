namespace MassTransitBenchmark.BusOutbox;

using System;
using MassTransit;


public class ServiceBusConfigureBusOutboxTransport :
    IConfigureBusOutboxTransport
{
    readonly ServiceBusHostSettings _hostSettings;
    readonly BusOutboxBenchmarkOptions _options;

    public ServiceBusConfigureBusOutboxTransport(ServiceBusHostSettings hostSettings, BusOutboxBenchmarkOptions options)
    {
        _hostSettings = hostSettings;
        _options = options;
    }

    public void Using(IBusRegistrationConfigurator configurator, Action<IBusRegistrationContext, IBusFactoryConfigurator> callback)
    {
        configurator.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(_hostSettings);

            callback(context, cfg);

            cfg.ConfigureEndpoints(context);
        });
    }
}
