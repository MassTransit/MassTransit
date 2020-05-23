namespace MassTransit.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Context;
    using EndpointConfigurators;
    using GreenPipes;
    using Logging;


    public abstract class BaseHostConfiguration<TReceiveEndpointConfiguration> :
        IHostConfiguration
        where TReceiveEndpointConfiguration : IReceiveEndpointConfiguration
    {
        readonly EndpointConfigurationObservable _endpointObservable;
        readonly IList<TReceiveEndpointConfiguration> _endpoints;
        ILogContext _logContext;

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

        public ILogContext LogContext
        {
            get => _logContext;
            set
            {
                _logContext = value;

                SendLogContext = value.CreateLogContext(LogCategoryName.Transport.Send);
                ReceiveLogContext = value.CreateLogContext(LogCategoryName.Transport.Receive);
            }
        }

        public ILogContext SendLogContext { get; private set; }
        public ILogContext ReceiveLogContext { get; private set; }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _endpointObservable.Connect(observer);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _endpoints.SelectMany(x => x.Validate());
        }

        public abstract IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IReceiveEndpointConfigurator> configure = null);

        public abstract IHost Build();

        protected void Add(TReceiveEndpointConfiguration configuration)
        {
            _endpoints.Add(configuration);
        }
    }
}
