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
        readonly EndpointConfigurationObservable _endpointObservable;
        readonly IList<TReceiveEndpointConfiguration> _endpoints;

        protected BaseHostConfiguration(IBusConfiguration busConfiguration)
        {
            BusConfiguration = busConfiguration;
            _endpoints = new List<TReceiveEndpointConfiguration>();
            _endpointObservable = new EndpointConfigurationObservable();
        }

        protected IEndpointConfigurationObserver Observers => _endpointObservable;

        protected IEnumerable<TReceiveEndpointConfiguration> Endpoints => _endpoints;

        public IBusConfiguration BusConfiguration { get; }

        public abstract Uri HostAddress { get; }

        public bool DeployTopologyOnly { get; set; }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _endpointObservable.Connect(observer);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _endpoints.SelectMany(x => x.Validate());
        }

        public abstract IBusHostControl Build();

        protected void Add(TReceiveEndpointConfiguration configuration)
        {
            _endpoints.Add(configuration);
        }
    }
}
