namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;


    public static class PublishEndpointExtensions
    {
        /// <summary>
        /// Publish a dynamically typed message initialized by a loosely typed dictionary of values. MassTransit will
        /// create and populate an object instance with the properties of the <paramref name="values" /> argument.
        /// </summary>
        /// <param name="messageType">The message type to publish</param>
        /// <param name="values">
        /// The dictionary of values to become hydrated and published under the type of the interface.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <param name="publishEndpoint"></param>
        public static Task Publish(this IPublishEndpoint publishEndpoint, Type messageType, object values, CancellationToken cancellationToken = default)
        {
            return PublishEndpointConverterCache.PublishInitializer(publishEndpoint, messageType, values, cancellationToken);
        }

        /// <summary>
        /// Publish a dynamically typed message initialized by a loosely typed dictionary of values. MassTransit will
        /// create and populate an object instance with the properties of the <paramref name="values" /> argument.
        /// </summary>
        /// <param name="messageType">The message type to publish</param>
        /// <param name="values">
        /// The dictionary of values to become hydrated and published under the type of the interface.
        /// </param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="publishEndpoint"></param>
        public static Task Publish(this IPublishEndpoint publishEndpoint, Type messageType, object values, IPipe<PublishContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return PublishEndpointConverterCache.PublishInitializer(publishEndpoint, messageType, values, pipe, cancellationToken);
        }
    }
}
