namespace MassTransit.Initializers
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// A message initializer that doesn't use the input
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IMessageInitializer<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Create a message context, using <paramref name="context"/> as a base for payloads, etc.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        InitializeContext<TMessage> Create(PipeContext context);

        /// <summary>
        /// Initialize the message, using the input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<InitializeContext<TMessage>> Initialize(object input, CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message, using the input
        /// </summary>
        /// <param name="context">An existing initialize message context</param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<InitializeContext<TMessage>> Initialize(InitializeContext<TMessage> context, object input);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="input">The input object</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TMessage> Send(ISendEndpoint endpoint, object input, CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="context"></param>
        /// <param name="input">The input object</param>
        /// <returns></returns>
        Task<TMessage> Send(ISendEndpoint endpoint, InitializeContext<TMessage> context, object input);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="input">The input object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TMessage> Send(ISendEndpoint endpoint, object input, IPipe<SendContext> pipe, CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="context">An existing context</param>
        /// <param name="input">The input object</param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        Task<TMessage> Send(ISendEndpoint endpoint, InitializeContext<TMessage> context, object input, IPipe<SendContext> pipe);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="input">The input object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TMessage> Send(ISendEndpoint endpoint, object input, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="endpoint">The destination endpoint</param>
        /// <param name="context">An existing context</param>
        /// <param name="input">The input object</param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        Task<TMessage> Send(ISendEndpoint endpoint, InitializeContext<TMessage> context, object input, IPipe<SendContext<TMessage>> pipe);
    }
}
