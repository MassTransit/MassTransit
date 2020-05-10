namespace MassTransit.Configuration
{
    using System;
    using Builders;
    using Configurators;
    using Context;
    using GreenPipes;
    using Logging;
    using Transports;


    public class ReceivePipeDispatcherConfiguration :
        ReceiverConfiguration,
        IReceiveEndpointConfigurator
    {
        readonly IReceiveEndpointConfiguration _endpointConfiguration;

        public ReceivePipeDispatcherConfiguration(IReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _endpointConfiguration = endpointConfiguration;
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointConfiguration.ConnectReceiveEndpointObserver(observer);
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
