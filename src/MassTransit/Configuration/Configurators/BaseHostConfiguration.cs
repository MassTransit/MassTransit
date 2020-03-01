namespace MassTransit.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using EndpointConfigurators;
    using GreenPipes;
    using Transports;


    public abstract class BaseHostConfiguration<TReceiveEndpointConfiguration> :
        IHostConfiguration
        where TReceiveEndpointConfiguration : IReceiveEndpointConfiguration
    {
        readonly IList<TReceiveEndpointConfiguration> _endpoints;
        readonly EndpointConfigurationObservable _endpointObservable;

        protected BaseHostConfiguration(IBusConfiguration busConfiguration)
        {
            BusConfiguration = busConfiguration;
            _endpoints = new List<TReceiveEndpointConfiguration>();
            _endpointObservable = new EndpointConfigurationObservable();
        }

        public IBusConfiguration BusConfiguration { get; }
        protected IEndpointConfigurationObserver Observers => _endpointObservable;

        protected void Add(TReceiveEndpointConfiguration configuration)
        {
            _endpoints.Add(configuration);
        }

        protected IEnumerable<TReceiveEndpointConfiguration> Endpoints => _endpoints;

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _endpointObservable.Connect(observer);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _endpoints.SelectMany(x => x.Validate());
        }

        public abstract Uri HostAddress { get; }

        public abstract IBusHostControl Build();
    }
}
