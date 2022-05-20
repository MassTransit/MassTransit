#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;
    using Observables;
    using Transports;
    using Util;


    public abstract class BaseHostConfiguration<TConfiguration, TConfigurator> :
        IHostConfiguration,
        IReceiveConfigurator<TConfigurator>
        where TConfiguration : IReceiveEndpointConfiguration
        where TConfigurator : IReceiveEndpointConfigurator
    {
        readonly ConsumeObservable _consumeObservers;
        readonly EndpointConfigurationObservable _endpointObservable;
        readonly PublishObservable _publishObservers;
        readonly ReceiveObservable _receiveObservers;
        readonly SendObservable _sendObservers;
        IList<TConfiguration> _endpoints;
        ILogContext? _logContext;

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

        public IBusConfiguration BusConfiguration { get; }

        public abstract Uri HostAddress { get; }

        public bool DeployTopologyOnly { get; set; }

        public ISendObserver SendObservers => _sendObservers;

        public ILogContext? LogContext
        {
            get => _logContext;
            set
            {
                _logContext = value;

                SendLogContext = value?.CreateLogContext(LogCategoryName.Transport.Send);
                ReceiveLogContext = value?.CreateLogContext(LogCategoryName.Transport.Receive);
            }
        }

        public ILogContext? ReceiveLogContext { get; private set; }
        public ILogContext? SendLogContext { get; private set; }

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
            return _endpoints.SelectMany(x => x.Validate()) ?? Enumerable.Empty<ValidationResult>();
        }

        public abstract IBusTopology Topology { get; }

        public abstract IRetryPolicy ReceiveTransportRetryPolicy { get; }

        public abstract IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IReceiveEndpointConfigurator>? configure);

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

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IReceiveEndpointConfigurator>? configureEndpoint)
        {
            ReceiveEndpoint(definition, endpointNameFormatter, (TConfigurator configuration) => configureEndpoint?.Invoke(configuration));
        }

        public abstract void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<TConfigurator>? configureEndpoint = null);

        public abstract void ReceiveEndpoint(string queueName, Action<TConfigurator> configureEndpoint);

        protected void ApplyEndpointDefinition(IReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
        {
            configurator.ConfigureConsumeTopology = definition.ConfigureConsumeTopology;

            configurator.ConcurrentMessageLimit = definition.ConcurrentMessageLimit;

            if (definition.PrefetchCount.HasValue)
                configurator.PrefetchCount = (ushort)definition.PrefetchCount.Value;
            else if (definition.ConcurrentMessageLimit.HasValue)
            {
                var concurrentMessageLimit = definition.ConcurrentMessageLimit.Value;

                var calculatedPrefetchCount = concurrentMessageLimit * 12 / 10;

                configurator.PrefetchCount = (ushort)calculatedPrefetchCount;
            }

            definition.Configure(configurator);
        }

        protected IEnumerable<TConfiguration> GetConfiguredEndpoints()
        {
            IList<TConfiguration> endpoints = _endpoints;

            _endpoints = new List<TConfiguration>();

            return endpoints;
        }

        protected void Add(TConfiguration configuration)
        {
            _endpoints.Add(configuration);
        }
    }
}
