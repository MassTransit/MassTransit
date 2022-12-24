namespace MassTransitBenchmark.BusOutbox;

using System;
using MassTransit;


public class InMemoryConfigureBusOutboxTransport :
    IConfigureBusOutboxTransport
{
    readonly BusOutboxBenchmarkOptions _options;
    readonly InMemoryOptionSet _optionSet;

    public InMemoryConfigureBusOutboxTransport(InMemoryOptionSet optionSet, BusOutboxBenchmarkOptions options)
    {
        _optionSet = optionSet;
        _options = options;
    }

    public void Using(IBusRegistrationConfigurator configurator, Action<IBusRegistrationContext, IBusFactoryConfigurator> callback)
    {
        configurator.UsingInMemory((context, cfg) =>
        {
            cfg.ConcurrentMessageLimit = _optionSet.TransportConcurrencyLimit;

            callback(context, cfg);

            cfg.ConfigureEndpoints(context);
        });
    }
}
