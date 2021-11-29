namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Net.Security;
    using MassTransit.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Transports;


    public class RabbitMqRegistrationBusFactory :
        TransportRegistrationBusFactory<IRabbitMqReceiveEndpointConfigurator>
    {
        readonly RabbitMqBusConfiguration _busConfiguration;
        readonly Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> _configure;

        public RabbitMqRegistrationBusFactory(Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> configure)
            : this(new RabbitMqBusConfiguration(new RabbitMqTopologyConfiguration(RabbitMqBusFactory.MessageTopology)), configure)
        {
        }

        RabbitMqRegistrationBusFactory(RabbitMqBusConfiguration busConfiguration,
            Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            var configurator = new RabbitMqBusFactoryConfigurator(_busConfiguration);

            var options = context.GetRequiredService<IOptionsMonitor<RabbitMqTransportOptions>>().Get(busName);

            configurator.Host(options.Host, options.Port, options.VHost, h =>
            {
                if (!string.IsNullOrWhiteSpace(options.User))
                    h.Username(options.User);

                if (!string.IsNullOrWhiteSpace(options.Pass))
                    h.Password(options.Pass);

                if (options.UseSsl)
                {
                    h.UseSsl(s =>
                    {
                        var sslOptions = context.GetRequiredService<IOptions<RabbitMqSslOptions>>().Value;

                        if (!string.IsNullOrWhiteSpace(sslOptions.ServerName))
                            s.ServerName = sslOptions.ServerName;
                        if (!string.IsNullOrWhiteSpace(sslOptions.CertPath))
                            s.CertificatePath = sslOptions.CertPath;
                        if (!string.IsNullOrWhiteSpace(sslOptions.CertPassphrase))
                            s.CertificatePassphrase = sslOptions.CertPassphrase;
                        s.UseCertificateAsAuthenticationIdentity = sslOptions.CertIdentity;

                        if (sslOptions.Trust)
                        {
                            s.AllowPolicyErrors(SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors
                                | SslPolicyErrors.RemoteCertificateNotAvailable);
                        }
                    });
                }
            });

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
