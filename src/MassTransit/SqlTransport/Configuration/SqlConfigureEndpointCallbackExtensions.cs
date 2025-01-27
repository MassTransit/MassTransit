namespace MassTransit;

using System;


public static class SqlConfigureEndpointCallbackExtensions
{
    /// <summary>
    /// Add an Azure Service Bus specific configure callback to the endpoint.
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    public static void AddSqlConfigureEndpointCallback(this IEndpointRegistrationConfigurator configurator,
        Action<IRegistrationContext, ISqlReceiveEndpointConfigurator> callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        configurator.AddConfigureEndpointCallback((context, cfg) =>
        {
            if (cfg is ISqlReceiveEndpointConfigurator sb)
                callback(context, sb);
        });
    }

    /// <summary>
    /// Add an Azure Service Bus specific configure callback for configured endpoints
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    public static void AddSqlConfigureEndpointsCallback(this IBusRegistrationConfigurator configurator, SqlConfigureEndpointsCallback callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        configurator.AddConfigureEndpointsCallback((context, name, cfg) =>
        {
            if (cfg is ISqlReceiveEndpointConfigurator sb)
                callback(context, name, sb);
        });
    }
}
