#nullable enable
namespace MassTransit;

using System;


public static class SqlReceiveEndpointConfigurationExtensions
{
    /// <summary>
    /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
    /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
    /// of this type (created automatically upon the first receiver binding).
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="configure"></param>
    public static void ReceiveEndpoint(this ISqlBusFactoryConfigurator configurator, Action<ISqlReceiveEndpointConfigurator>? configure = null)
    {
        configurator.ReceiveEndpoint(new TemporaryEndpointDefinition(), DefaultEndpointNameFormatter.Instance, configure);
    }

    /// <summary>
    /// Declare a receive endpoint using the endpoint <paramref name="definition"/>.
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="definition"></param>
    /// <param name="configure"></param>
    public static void ReceiveEndpoint(this ISqlBusFactoryConfigurator configurator, IEndpointDefinition definition,
        Action<ISqlReceiveEndpointConfigurator>? configure = null)
    {
        configurator.ReceiveEndpoint(definition, DefaultEndpointNameFormatter.Instance, configure);
    }
}
