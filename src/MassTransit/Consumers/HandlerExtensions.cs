namespace MassTransit
{
    using System;
    using Configuration;


    public static class HandlerExtensions
    {
        /// <summary>
        /// Adds a handler to the receive endpoint with additional configuration specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="handler"></param>
        /// <param name="configure"></param>
        public static void Handler<T>(this IReceiveEndpointConfigurator configurator, MessageHandler<T> handler,
            Action<IHandlerConfigurator<T>> configure = null)
            where T : class
        {
            var handlerConfigurator = new HandlerConfigurator<T>(handler, configurator);

            configure?.Invoke(handlerConfigurator);

            configurator.AddEndpointSpecification(handlerConfigurator);
        }

        /// <summary>
        /// Adds a message handler to the service bus for handling a specific type of message
        /// </summary>
        /// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
        /// <param name="connector"></param>
        /// <param name="handler">
        /// The callback to invoke when messages of the specified type arrive at the service bus
        /// </param>
        /// <param name="configurator"></param>
        public static ConnectHandle ConnectHandler<T>(this IConsumePipeConnector connector, MessageHandler<T> handler,
            IBuildPipeConfigurator<ConsumeContext<T>> configurator = null)
            where T : class
        {
            return HandlerConnectorCache<T>.Connector.ConnectHandler(connector, handler, configurator);
        }

        /// <summary>
        /// Subscribe a request handler to the bus's endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="requestId"></param>
        /// <param name="handler"></param>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectRequestHandler<T>(this IRequestPipeConnector connector, Guid requestId, MessageHandler<T> handler,
            IBuildPipeConfigurator<ConsumeContext<T>> configurator)
            where T : class
        {
            return HandlerConnectorCache<T>.Connector.ConnectRequestHandler(connector, requestId, handler, configurator);
        }
    }
}
