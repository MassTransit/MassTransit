namespace MassTransit
{
    using System;


    /// <summary>
    /// The client factory context, which contains multiple interfaces and properties used by clients
    /// </summary>
    public interface ClientFactoryContext :
        IConsumePipeConnector,
        IRequestPipeConnector
    {
        /// <summary>
        /// Default timeout for requests
        /// </summary>
        RequestTimeout DefaultTimeout { get; }

        /// <summary>
        /// The address used for responses to messages sent by this client
        /// </summary>
        Uri ResponseAddress { get; }

        /// <summary>
        /// Returns an endpoint to which requests are sent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRequestSendEndpoint<T> GetRequestEndpoint<T>(ConsumeContext? consumeContext = default)
            where T : class;

        /// <summary>
        /// Returns an endpoint to which requests are sent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRequestSendEndpoint<T> GetRequestEndpoint<T>(Uri destinationAddress, ConsumeContext? consumeContext = default)
            where T : class;
    }
}
