namespace MassTransitBenchmark.BusOutbox;

using System;
using MassTransit;


public interface IConfigureBusOutboxTransport
{
    void Using(IBusRegistrationConfigurator configurator, Action<IBusRegistrationContext, IBusFactoryConfigurator> callback);
}
