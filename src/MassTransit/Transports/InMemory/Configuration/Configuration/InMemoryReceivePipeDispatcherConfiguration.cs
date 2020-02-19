namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using Context;
    using GreenPipes;
    using Logging;
    using MassTransit.Builders;
    using MassTransit.Configuration;
    using MassTransit.Configurators;


    public class InMemoryReceivePipeDispatcherConfiguration :
        ReceiverConfiguration,
        IInMemoryReceiveEndpointConfigurator
    {
        readonly IInMemoryReceiveEndpointConfiguration _endpointConfiguration;

        public InMemoryReceivePipeDispatcherConfiguration(IInMemoryReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _endpointConfiguration = endpointConfiguration;

            InputAddress = endpointConfiguration.InputAddress;

            RethrowExceptions();
            ThrowOnDeadLetter();
        }

        public Uri InputAddress { get; }

        public void AddDependency(IReceiveEndpointObserverConnector connector)
        {
            _endpointConfiguration.Configurator.AddDependency(connector);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointConfiguration.ConnectReceiveEndpointObserver(observer);
        }

        public int ConcurrencyLimit
        {
            set => _endpointConfiguration.Configurator.ConcurrencyLimit = value;
        }

        public IReceivePipeDispatcher Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new ReceiveEndpointBuilder(_endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                var logContext = LogContext.Current.CreateLogContext(LogCategoryName.Transport.Receive);

                return new ReceivePipeDispatcher(_endpointConfiguration.CreateReceivePipe(), _endpointConfiguration.ReceiveObservers, logContext);
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred during handler creation", ex);
            }
        }
    }
}
