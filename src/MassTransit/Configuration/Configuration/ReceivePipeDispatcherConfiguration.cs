namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Transports;


    public class ReceivePipeDispatcherConfiguration :
        ReceiverConfiguration,
        IReceiveEndpointConfigurator
    {
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly IHostConfiguration _hostConfiguration;

        public ReceivePipeDispatcherConfiguration(IHostConfiguration hostConfiguration, IReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointConfiguration.ConnectReceiveEndpointObserver(observer);
        }

        public IReceivePipeDispatcher Build()
        {
            IReadOnlyList<ValidationResult> result = Validate().ThrowIfContainsFailure($"{GetType().Name} configuration is invalid:");

            try
            {
                var builder = new ReceiveEndpointBuilder(_endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return new ReceivePipeDispatcher(_endpointConfiguration.CreateReceivePipe(), _endpointConfiguration.ReceiveObservers, _hostConfiguration,
                    _endpointConfiguration.InputAddress);
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred during mediator creation", ex);
            }
        }
    }
}
