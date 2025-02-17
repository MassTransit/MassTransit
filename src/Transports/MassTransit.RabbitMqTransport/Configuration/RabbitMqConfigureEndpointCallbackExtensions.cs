namespace MassTransit;

using System;


public static class RabbitMqConfigureEndpointCallbackExtensions
{
    /// <summary>
    /// Add a RabbitMQ specific configure callback to the endpoint.
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    public static void AddRabbitMqConfigureEndpointCallback(this IEndpointRegistrationConfigurator configurator,
        Action<IRegistrationContext, IRabbitMqReceiveEndpointConfigurator> callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        configurator.AddConfigureEndpointCallback((context, cfg) =>
        {
            if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
                callback(context, rmq);
        });
    }

    /// <summary>
    /// Add a RabbitMQ specific configure callback for configured endpoints
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="callback"></param>
    public static void AddRabbitMqConfigureEndpointsCallback(this IBusRegistrationConfigurator configurator, RabbitMqConfigureEndpointsCallback callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        configurator.AddConfigureEndpointsCallback((context, name, cfg) =>
        {
            if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
                callback(context, name, rmq);
        });
    }
}
