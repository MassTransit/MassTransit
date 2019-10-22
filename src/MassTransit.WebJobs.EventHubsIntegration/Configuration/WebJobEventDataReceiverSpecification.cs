namespace MassTransit.WebJobs.EventHubsIntegration.Configuration
{
    using System;
    using System.Threading;
    using Azure.ServiceBus.Core.Builders;
    using Azure.ServiceBus.Core.Configurators;
    using Configurators;
    using Context;
    using Contexts;
    using MassTransit.Configuration;
    using Microsoft.Azure.WebJobs;


    public class WebJobEventDataReceiverSpecification :
        MessageReceiverSpecification,
        IWebJobReceiverConfigurator,
        IWebJobHandlerFactory
    {
        readonly IBinder _binder;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        CancellationToken _cancellationToken;

        public WebJobEventDataReceiverSpecification(IBinder binder, IReceiveEndpointConfiguration endpointConfiguration,
            CancellationToken cancellationToken = default)
            : base(endpointConfiguration)
        {
            _binder = binder;
            _endpointConfiguration = endpointConfiguration;
            _cancellationToken = cancellationToken;
        }

        public CancellationToken CancellationToken
        {
            set => _cancellationToken = value;
        }

        protected virtual ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return new WebJobEventDataReceiveEndpointContext(_endpointConfiguration, _binder, _cancellationToken);
        }

        public IEventDataReceiver Build()
        {
            var result = BusConfigurationResult.CompileResults(Validate());

            try
            {
                var builder = new MessageReceiverBuilder(_endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return new EventDataReceiver(InputAddress, CreateReceiveEndpointContext());
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred during handler creation", ex);
            }
        }
    }
}
