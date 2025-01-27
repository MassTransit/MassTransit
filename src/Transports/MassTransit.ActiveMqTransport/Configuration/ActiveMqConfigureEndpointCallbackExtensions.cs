namespace MassTransit;

using System;


public static class ActiveMqConfigureEndpointCallbackExtensions
{
    /// <summary>
    /// Add an Azure Service Bus specific configure callback to the endpoint.
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    public static void AddActiveMqConfigureEndpointCallback(this IEndpointRegistrationConfigurator configurator,
        Action<IRegistrationContext, IActiveMqReceiveEndpointConfigurator> callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        configurator.AddConfigureEndpointCallback((context, cfg) =>
        {
            if (cfg is IActiveMqReceiveEndpointConfigurator sb)
                callback(context, sb);
        });
    }

    /// <summary>
    /// Add an Azure Service Bus specific configure callback for configured endpoints
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    public static void AddActiveMqConfigureEndpointsCallback(this IBusRegistrationConfigurator configurator, ActiveMqConfigureEndpointsCallback callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        configurator.AddConfigureEndpointsCallback((context, name, cfg) =>
        {
            if (cfg is IActiveMqReceiveEndpointConfigurator sb)
                callback(context, name, sb);
        });
    }
}
