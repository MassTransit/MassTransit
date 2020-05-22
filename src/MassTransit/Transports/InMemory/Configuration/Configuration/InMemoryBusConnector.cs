namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using Builders;
    using Context;
    using MassTransit.Configurators;
    using Registration;


    public class InMemoryBusConnector :
        IBusConnector
    {
        readonly IInMemoryHostConfiguration _hostConfiguration;

        public InMemoryBusConnector(IInMemoryHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
        }

        public ReceiveEndpointContext CreateReceiveEndpointContext(string entityName, Action<IReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = _hostConfiguration.CreateReceiveEndpointConfiguration(entityName);
            var receiverConfiguration = new BusConnectorReceiverConfiguration(endpointConfiguration);

            configure?.Invoke(receiverConfiguration);

            var result = BusConfigurationResult.CompileResults(receiverConfiguration.Validate());
            try
            {
                var builder = new InMemoryReceiveEndpointBuilder(_hostConfiguration.Proxy, endpointConfiguration);
                receiverConfiguration.Configure(builder);

                return builder.CreateReceiveEndpointContext();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(result, "An exception occurred creating the EventDataReceiver", ex);
            }
        }
    }
}
