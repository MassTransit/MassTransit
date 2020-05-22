namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using Builders;
    using Context;
    using MassTransit.Configurators;
    using Registration;


    public class RabbitMqBusConnector :
        IBusConnector
    {
        readonly IRabbitMqHostConfiguration _hostConfiguration;

        public RabbitMqBusConnector(IRabbitMqHostConfiguration hostConfiguration)
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
                var builder = new RabbitMqReceiveEndpointBuilder(_hostConfiguration.Proxy, endpointConfiguration);
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
