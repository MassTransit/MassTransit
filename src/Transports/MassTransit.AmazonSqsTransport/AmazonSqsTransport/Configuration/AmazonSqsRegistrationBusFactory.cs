namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Amazon;
    using Amazon.Runtime;
    using MassTransit.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Transports;


    public class AmazonSqsRegistrationBusFactory :
        TransportRegistrationBusFactory<IAmazonSqsReceiveEndpointConfigurator>
    {
        readonly AmazonSqsBusConfiguration _busConfiguration;
        readonly Action<IBusRegistrationContext, IAmazonSqsBusFactoryConfigurator> _configure;

        public AmazonSqsRegistrationBusFactory(Action<IBusRegistrationContext, IAmazonSqsBusFactoryConfigurator> configure)
            : this(new AmazonSqsBusConfiguration(new AmazonSqsTopologyConfiguration(AmazonSqsBusFactory.MessageTopology)), configure)
        {
        }

        AmazonSqsRegistrationBusFactory(AmazonSqsBusConfiguration busConfiguration,
            Action<IBusRegistrationContext, IAmazonSqsBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            var configurator = new AmazonSqsBusFactoryConfigurator(_busConfiguration);

            var options = context.GetRequiredService<IOptionsMonitor<AmazonSqsTransportOptions>>().Get(busName);
            if (!string.IsNullOrWhiteSpace(options.Region))
            {
                var regionEndpoint = RegionEndpoint.GetBySystemName(options.Region);

                configurator.Host(regionEndpoint.SystemName, h =>
                {
                    if (!string.IsNullOrWhiteSpace(options.Scope))
                        h.Scope(options.Scope);

                    if (!string.IsNullOrWhiteSpace(options.AccessKey) && !string.IsNullOrWhiteSpace(options.SecretKey))
                    {
                        h.AccessKey(options.AccessKey);
                        h.SecretKey(options.SecretKey);
                    }
                    else
                        h.Credentials(FallbackCredentialsFactory.GetCredentials());
                });
            }

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
