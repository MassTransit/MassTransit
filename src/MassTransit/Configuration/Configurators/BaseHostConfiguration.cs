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
    using Pipeline.Observables;
    using Topology;
    using Util;


    public abstract class BaseHostConfiguration<TReceiveEndpointConfiguration> :
        IHostConfiguration
        where TReceiveEndpointConfiguration : IReceiveEndpointConfiguration
    {
        readonly ConsumeObservable _consumeObservers;
        readonly EndpointConfigurationObservable _endpointObservable;
        readonly IList<TReceiveEndpointConfiguration> _endpoints;
        readonly PublishObservable _publishObservers;
        readonly ReceiveObservable _receiveObservers;
        readonly SendObservable _sendObservers;
        ILogContext _logContext;

        protected BaseHostConfiguration(IBusConfiguration busConfiguration)
        {
            BusConfiguration = busConfiguration;
            _endpoints = new List<TReceiveEndpointConfiguration>();

            _endpointObservable = new EndpointConfigurationObservable();

            _receiveObservers = new ReceiveObservable();
            _consumeObservers = new ConsumeObservable();
            _publishObservers = new PublishObservable();
            _sendObservers = new SendObservable();
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
            }
        }

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

        protected void Add(TReceiveEndpointConfiguration configuration)
        {
            _endpoints.Add(configuration);
        }
    }
}
