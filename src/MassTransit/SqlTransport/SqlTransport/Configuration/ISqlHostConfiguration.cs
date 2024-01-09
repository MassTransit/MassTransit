#nullable enable
namespace MassTransit.SqlTransport.Configuration
{
    using System;
    using MassTransit.Configuration;


    public interface ISqlHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<ISqlReceiveEndpointConfigurator>
    {
        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        SqlHostSettings Settings { get; set; }

        new ISqlBusTopology Topology { get; }

        /// <summary>
        /// Apply the endpoint definition to the receive endpoint configurator
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        void ApplyEndpointDefinition(ISqlReceiveEndpointConfigurator configurator, IEndpointDefinition definition);

        ISqlReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<ISqlReceiveEndpointConfigurator>? configure = null);

        ISqlReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(SqlReceiveSettings settings,
            ISqlEndpointConfiguration endpointConfiguration, Action<ISqlReceiveEndpointConfigurator>? configure = null);
    }
}
