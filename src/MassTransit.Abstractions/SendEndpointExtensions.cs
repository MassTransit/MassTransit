namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;


    public static class SendEndpointExtensions
    {
        /// <summary>
        /// Send a dynamically typed message initialized by a loosely typed dictionary of values. MassTransit will
        /// create and populate an object instance with the properties of the <paramref name="values" /> argument.
        /// </summary>
        /// <param name="messageType">The message type to publish</param>
        /// <param name="values">
        /// The dictionary of values to become hydrated and published under the type of the interface.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <param name="publishEndpoint"></param>
        public static Task Send(this ISendEndpoint publishEndpoint, Type messageType, object values, CancellationToken cancellationToken = default)
        {
            return SendEndpointConverterCache.SendInitializer(publishEndpoint, messageType, values, cancellationToken);
        }

        /// <summary>
        /// Send a dynamically typed message initialized by a loosely typed dictionary of values. MassTransit will
        /// create and populate an object instance with the properties of the <paramref name="values" /> argument.
        /// </summary>
        /// <param name="messageType">The message type to publish</param>
        /// <param name="values">
        /// The dictionary of values to become hydrated and published under the type of the interface.
        /// </param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="publishEndpoint"></param>
        public static Task Send(this ISendEndpoint publishEndpoint, Type messageType, object values, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return SendEndpointConverterCache.SendInitializer(publishEndpoint, messageType, values, pipe, cancellationToken);
        }
    }
}
