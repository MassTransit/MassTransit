namespace MassTransit.Builders
{
    using System;
    using Configuration;
    using Util;


    public class ConfigurationBusBuilder :
        IBusBuilder
    {
        readonly IReceiveEndpointConfiguration _busEndpointConfiguration;
        readonly IBusConfiguration _busConfiguration;

        public ConfigurationBusBuilder(IBusConfiguration busConfiguration, IReceiveEndpointConfiguration busReceiveEndpointConfiguration)
        {
            _busConfiguration = busConfiguration;
            _busEndpointConfiguration = busReceiveEndpointConfiguration;
        }

        public IBusControl Build()
        {
            try
            {
                var host = _busConfiguration.HostConfiguration.Build();

                var bus = new MassTransitBus(host, _busConfiguration.BusObservers, _busEndpointConfiguration);

                TaskUtil.Await(() => _busConfiguration.BusObservers.PostCreate(bus));

                return bus;
            }
            catch (Exception exception)
            {
                TaskUtil.Await(() => _busConfiguration.BusObservers.CreateFaulted(exception));

                throw;
            }
        }
    }
}
