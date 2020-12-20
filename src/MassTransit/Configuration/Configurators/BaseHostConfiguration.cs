namespace MassTransit.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Context;
    using EndpointConfigurators;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using Pipeline.Observables;
    using Topology;
    using Util;


    public abstract class BaseHostConfiguration<TConfiguration, TConfigurator> :
        IHostConfiguration,
        IReceiveConfigurator<TConfigurator>
        where TConfiguration : IReceiveEndpointConfiguration
        where TConfigurator : IReceiveEndpointConfigurator
    {
        readonly ConsumeObservable _consumeObservers;
        readonly EndpointConfigurationObservable _endpointObservable;
        readonly IList<TConfiguration> _endpoints;
        readonly PublishObservable _publishObservers;
        readonly ReceiveObservable _receiveObservers;
        readonly SendObservable _sendObservers;
        ILogContext _logContext;

        protected BaseHostConfiguration(IBusConfiguration busConfiguration)
        {
            BusConfiguration = busConfiguration;
            _endpoints = new List<TConfiguration>();

            _endpointObservable = new EndpointConfigurationObservable();

            _receiveObservers = new ReceiveObservable();
            _consumeObservers = new ConsumeObservable();
            _publishObservers = new PublishObservable();
            _sendObservers = new SendObservable();
        }

        protected IEndpointConfigurationObserver Observers => _endpointObservable;

        protected IEnumerable<TConfiguration> Endpoints => _endpoints;

        public abstract IAgent Agent { get; }

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

        public ILogContext ReceiveLogContext { get; private set; }
        public ILogContext SendLogContext { get; private set; }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _endpointObservable.Connect(observer);
        }

        public ConnectHandle ConnectReceiveEndpointContext(ReceiveEndpointContext context)
        {
            var consume = context.ReceivePipe.ConnectConsumeObserver(_consumeObservers);
            var receive = context.ConnectReceiveObserver(_receiveObservers);
            var publish = context.ConnectPublishObserver(_publishObservers);
            var send = context.ConnectSendObserver(_sendObservers);

            return new MultipleConnectHandle(consume, receive, publish, send);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return _endpoints.SelectMany(x => x.Validate());
        }

        public abstract IHostTopology HostTopology { get; }

        public abstract IRetryPolicy ReceiveTransportRetryPolicy { get; }

        public abstract IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IReceiveEndpointConfigurator> configure = null);

        public abstract IHost Build();

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _consumeObservers.Connect(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservers.Connect(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, (TConfigurator configuration) => configureEndpoint(configuration));
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(definition, endpointNameFormatter, (TConfigurator configuration) => configureEndpoint(configuration));
        }

        public abstract void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<TConfigurator> configureEndpoint = null);

        public abstract void ReceiveEndpoint(string queueName, Action<TConfigurator> configureEndpoint);

        protected void Add(TConfiguration configuration)
        {
            _endpoints.Add(configuration);
        }
    }
}
