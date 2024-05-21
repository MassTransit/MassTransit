#nullable enable
namespace MassTransit.SqlTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using Transports;


    public class SqlRegistrationBusFactory :
        TransportRegistrationBusFactory<ISqlReceiveEndpointConfigurator>
    {
        readonly SqlBusConfiguration _busConfiguration;
        readonly Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? _configure;

        public SqlRegistrationBusFactory(Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? configure)
            : this(new SqlBusConfiguration(new SqlTopologyConfiguration(SqlBusFactory.CreateMessageTopology())), configure)
        {
        }

        SqlRegistrationBusFactory(SqlBusConfiguration busConfiguration, Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            var configurator = new SqlBusFactoryConfigurator(_busConfiguration);

            configurator.UseRawJsonSerializer(RawSerializerOptions.CopyHeaders, true);

            // var options = context.GetRequiredService<IOptionsMonitor<DbTransportOptions>>().Get(busName);
            //
            // configurator.Host(options.Host, options.Port, options.VHost, h =>
            // {
            //     if (!string.IsNullOrWhiteSpace(options.User))
            //         h.Username(options.User);
            //
            //     if (!string.IsNullOrWhiteSpace(options.Pass))
            //         h.Password(options.Pass);
            // });

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
