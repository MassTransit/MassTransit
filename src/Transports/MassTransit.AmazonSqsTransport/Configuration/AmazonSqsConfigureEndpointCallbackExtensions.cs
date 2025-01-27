namespace MassTransit;

using System;


public static class AmazonSqsConfigureEndpointCallbackExtensions
{
    /// <summary>
    /// Add an Azure Service Bus specific configure callback to the endpoint.
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    public static void AddAmazonSqsConfigureEndpointCallback(this IEndpointRegistrationConfigurator configurator,
        Action<IRegistrationContext, IAmazonSqsReceiveEndpointConfigurator> callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        configurator.AddConfigureEndpointCallback((context, cfg) =>
        {
            if (cfg is IAmazonSqsReceiveEndpointConfigurator sb)
                callback(context, sb);
        });
    }

    /// <summary>
    /// Add an Azure Service Bus specific configure callback for configured endpoints
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    public static void AddAmazonSqsConfigureEndpointsCallback(this IBusRegistrationConfigurator configurator, AmazonSqsConfigureEndpointsCallback callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        configurator.AddConfigureEndpointsCallback((context, name, cfg) =>
        {
            if (cfg is IAmazonSqsReceiveEndpointConfigurator sb)
                callback(context, name, sb);
        });
    }
}
