namespace MassTransit.Builders
{
    using System;
    using Configuration;
    using EndpointSpecifications;
    using Pipeline.Observables;
    using Util;


    public class ConfigurationBusBuilder :
        IBusBuilder
    {
        readonly IReceiveEndpointConfiguration _busEndpointConfiguration;
        readonly ConfigurationReceiveEndpointSpecification _busEndpointSpecification;
        readonly BusObservable _busObservable;
        readonly IBusConfiguration _configuration;

        public ConfigurationBusBuilder(IBusConfiguration configuration, IReceiveEndpointConfiguration busReceiveEndpointConfiguration,
            BusObservable busObservable)
        {
            _busEndpointSpecification = new ConfigurationReceiveEndpointSpecification(busReceiveEndpointConfiguration);
            _configuration = configuration;
            _busEndpointConfiguration = busReceiveEndpointConfiguration;

            _busObservable = busObservable;
        }

        public IBusControl Build()
        {
            try
            {
                _busEndpointSpecification.Apply(this);

                var bus = new MassTransitBus(_busEndpointConfiguration.InputAddress, _busEndpointConfiguration.ConsumePipe,
                    _busEndpointSpecification.SendEndpointProvider, _busEndpointSpecification.PublishEndpointProvider, _configuration.Hosts, _busObservable);

                TaskUtil.Await(() => _busObservable.PostCreate(bus));

                return bus;
            }
            catch (Exception exception)
            {
                TaskUtil.Await(() => _busObservable.CreateFaulted(exception));

                throw;
            }
        }
    }
}
