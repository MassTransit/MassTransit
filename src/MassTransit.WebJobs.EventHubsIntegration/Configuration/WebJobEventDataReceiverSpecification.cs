namespace MassTransit.WebJobs.EventHubsIntegration.Configuration
{
    using System;
    using System.Threading;
    using Azure.ServiceBus.Core.Configuration;
    using Azure.ServiceBus.Core.Configurators;
    using Builders;
    using Configurators;
    using Context;
    using Contexts;
    using MassTransit.Configuration;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;


    public class WebJobEventDataReceiverSpecification :
        AzureReceiverConfiguration,
        IWebJobHandlerFactory
    {
        readonly IBinder _binder;
        readonly CancellationToken _cancellationToken;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;

        public WebJobEventDataReceiverSpecification(IBinder binder, ILogger logger, IServiceBusReceiveEndpointConfiguration endpointConfiguration,
            CancellationToken cancellationToken = default)
            : base(endpointConfiguration)
        {
            _binder = binder;
            _endpointConfiguration = endpointConfiguration;
            _cancellationToken = cancellationToken;

            LogContext.ConfigureCurrentLogContext(logger);

            RethrowExceptions();
            ThrowOnDeadLetter();
        }

        protected virtual ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return new WebJobEventDataReceiveEndpointContext(_endpointConfiguration, InputAddress, _binder, _cancellationToken);
        }

        public IEventDataReceiver Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new ReceiveEndpointBuilder(_endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return new EventDataReceiver(CreateReceiveEndpointContext());
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating the EventDataReceiver", ex);
            }
        }
    }
}
